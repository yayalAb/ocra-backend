using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
// Customer create command with CustomerResponse
public class UpdateAdoptionCommand : IRequest<UpdateAdoptionCommandResponse>
{
    public AddAdoptionRequest Adoption { get; set; }
}


public class UpdateAdoptionCommandHandler : IRequestHandler<UpdateAdoptionCommand, UpdateAdoptionCommandResponse>
{
    private readonly IAdoptionEventRepository _adoptionEventRepository;
    private readonly IPersonalInfoRepository _personalInfoRepository;
    private readonly IEventDocumentService _eventDocumentService;
    private readonly IFileService _fileService;


    public UpdateAdoptionCommandHandler(IEventDocumentService eventDocumentService, IAdoptionEventRepository adoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
    {
        _adoptionEventRepository = adoptionEventRepository;
        _personalInfoRepository = personalInfoRepository;
        _fileService = fileService;

        _eventDocumentService = eventDocumentService;
    }
    public async Task<UpdateAdoptionCommandResponse> Handle(UpdateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse();

        var validator = new CreateAdoptionCommandValidetor(_adoptionEventRepository);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.Errors.Count > 0)
        {
            UpdateAdoptionCommandResponse.Success = false;
            UpdateAdoptionCommandResponse.ValidationErrors = new List<string>();
            foreach (var error in validationResult.Errors)
                UpdateAdoptionCommandResponse.ValidationErrors.Add(error.ErrorMessage);
            UpdateAdoptionCommandResponse.Message = UpdateAdoptionCommandResponse.ValidationErrors[0];
        }
        else if (UpdateAdoptionCommandResponse.Success)
        {
            try
            {
                //supporting docs cant be updated only new (one without id) are created
                var supportingDocs = request.Adoption.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                var examptionsupportingDocs = request.Adoption.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                request.Adoption.Event.EventSupportingDocuments = null;
                if (request.Adoption.Event.PaymentExamption != null)
                {
                    request.Adoption.Event.PaymentExamption.SupportingDocuments = null;
                }
                //////
                request.Adoption.Event.EventType = "Adoption";
                var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request.Adoption);
                if (adoptionEvent.AdoptiveFather?.Id != null && adoptionEvent.AdoptiveFather?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.AdoptiveFather.Id);
                    selectedperson.NationalId = adoptionEvent.AdoptiveFather?.NationalId;
                    selectedperson.NationalityLookupId = adoptionEvent.AdoptiveFather?.NationalityLookupId;
                    selectedperson.ReligionLookupId = adoptionEvent.AdoptiveFather?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = adoptionEvent.AdoptiveFather?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = adoptionEvent.AdoptiveFather?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = adoptionEvent.AdoptiveFather?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = adoptionEvent.AdoptiveFather?.NationLookupId;
                    adoptionEvent.AdoptiveFather = selectedperson;
                }
                if (adoptionEvent.AdoptiveMother?.Id != null && adoptionEvent.AdoptiveMother?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.AdoptiveMother.Id);
                    selectedperson.NationalId = adoptionEvent.AdoptiveMother?.NationalId;
                    selectedperson.NationalityLookupId = adoptionEvent.AdoptiveMother?.NationalityLookupId;
                    selectedperson.ReligionLookupId = adoptionEvent.AdoptiveMother?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = adoptionEvent.AdoptiveMother?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = adoptionEvent.AdoptiveMother?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = adoptionEvent.AdoptiveMother?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = adoptionEvent.AdoptiveMother?.NationLookupId;
                    adoptionEvent.AdoptiveMother = selectedperson;
                }
                if (adoptionEvent.Event.EventOwener?.Id != null && adoptionEvent.Event.EventOwener?.Id != Guid.Empty)
                {
                    PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.Event.EventOwener.Id);
                    selectedperson.NationalId = adoptionEvent.Event?.EventOwener?.NationalId;
                    selectedperson.NationalityLookupId = adoptionEvent.Event?.EventOwener?.NationalityLookupId;
                    selectedperson.ReligionLookupId = adoptionEvent.Event?.EventOwener?.ReligionLookupId;
                    selectedperson.EducationalStatusLookupId = adoptionEvent.Event?.EventOwener?.EducationalStatusLookupId;
                    selectedperson.TypeOfWorkLookupId = adoptionEvent.Event?.EventOwener?.TypeOfWorkLookupId;
                    selectedperson.MarriageStatusLookupId = adoptionEvent.Event?.EventOwener?.MarriageStatusLookupId;
                    selectedperson.NationLookupId = adoptionEvent.Event?.EventOwener?.NationLookupId;
                    adoptionEvent.Event.EventOwener = selectedperson;
                }
                _adoptionEventRepository.EFUpdate(adoptionEvent);
                await _adoptionEventRepository.SaveChangesAsync(cancellationToken);
                var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, adoptionEvent.EventId, adoptionEvent.Event.PaymentExamption.Id, cancellationToken);
                var personIds = new PersonIdObj
                {
                    MotherId = adoptionEvent.AdoptiveMother.Id,
                    FatherId = adoptionEvent.AdoptiveFather.Id,
                    ChildId = adoptionEvent.Event.EventOwener.Id
                };
                var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                _eventDocumentService.savePhotos(separatedDocs.userPhotos);

                _eventDocumentService.saveSupportingDocuments(adoptionEvent.Event.EventSupportingDocuments, adoptionEvent.Event.PaymentExamption.SupportingDocuments, "Adoption");
                UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse { Message = "Adoption Event Updated Successfully" };
            }
            catch (Exception ex)
            {
                UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
        return UpdateAdoptionCommandResponse;
    }
}
