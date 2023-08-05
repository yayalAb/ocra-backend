using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AppDiv.CRVS.Application.Contracts.Request.AdoptionPersonalINformationRequest;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
// Customer create command with CustomerResponse
public class UpdateAdoptionCommand : IRequest<UpdateAdoptionCommandResponse>
{
    public Guid? Id { get; set; }
    public Guid? BeforeAdoptionAddressId { get; set; }
    public string? BirthCertificateId { get; set; }
    public LanguageModel ApprovedName { get; set; }
    public LanguageModel? Reason { get; set; }
    public virtual AddAdoptionPersonalInfoRequest AdoptiveMother { get; set; }
    public AddAdoptionPersonalInfoRequest AdoptiveFather { get; set; }
    public virtual AddCourtCaseRequest CourtCase { get; set; }
    public virtual AddAdoptionEventRequest Event { get; set; }
    public bool IsFromCommand { get; set; } = false;
    public bool ValidateFirst { get; set; } = false;

}


public class UpdateAdoptionCommandHandler : IRequestHandler<UpdateAdoptionCommand, UpdateAdoptionCommandResponse>
{
    private readonly IAdoptionEventRepository _adoptionEventRepository;
    private readonly IPersonalInfoRepository _personalInfoRepository;
    private readonly IEventDocumentService _eventDocumentService;
    private readonly IFileService _fileService;
    private readonly ILookupRepository _LookupsRepo;


    public UpdateAdoptionCommandHandler(ILookupRepository LookupsRepo, IEventDocumentService eventDocumentService, IAdoptionEventRepository adoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
    {
        _adoptionEventRepository = adoptionEventRepository;
        _personalInfoRepository = personalInfoRepository;
        _fileService = fileService;
        _LookupsRepo = LookupsRepo;
        _eventDocumentService = eventDocumentService;
    }
    public async Task<UpdateAdoptionCommandResponse> Handle(UpdateAdoptionCommand request, CancellationToken cancellationToken)
    {
        var UpdateAdoptionCommandResponse = new UpdateAdoptionCommandResponse();
        request.Event.EventOwener.MiddleName = request?.AdoptiveFather?.FirstName;
        request.Event.EventOwener.LastName = request?.AdoptiveFather?.MiddleName;
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
            if (request.ValidateFirst == true)
            {
                UpdateAdoptionCommandResponse.Created(entity: "Adoption", message: "Valid Input.");
                return UpdateAdoptionCommandResponse;
            }
            try
            {
                request.Event.EventType = "Adoption";
                request.AdoptiveFather.SexLookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                        || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                        || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                    .Select(l => l.Id).FirstOrDefault();

                request.AdoptiveMother.SexLookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                                        .Select(l => l.Id).FirstOrDefault();
                request.Event.EventDateEt = request.CourtCase.ConfirmedDateEt;

                var supportingDocs = request.Event.EventSupportingDocuments?.Where(doc => doc.Id == null).ToList();
                var examptionsupportingDocs = request.Event.PaymentExamption?.SupportingDocuments?.Where(doc => doc.Id == null).ToList();
                var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request);
                adoptionEvent.Event.EventAddressId = adoptionEvent.CourtCase.Court.AddressId;

                // if (adoptionEvent.AdoptiveFather?.Id != null && adoptionEvent.AdoptiveFather?.Id != Guid.Empty)
                // {
                //     PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.AdoptiveFather.Id);
                //     selectedperson.NationalId = adoptionEvent.AdoptiveFather?.NationalId;
                //     selectedperson.NationalityLookupId = adoptionEvent.AdoptiveFather?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = adoptionEvent.AdoptiveFather?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = adoptionEvent.AdoptiveFather?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = adoptionEvent.AdoptiveFather?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = adoptionEvent.AdoptiveFather?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = adoptionEvent.AdoptiveFather?.NationLookupId;
                //     adoptionEvent.AdoptiveFather = selectedperson;
                // }
                // if (adoptionEvent.AdoptiveMother?.Id != null && adoptionEvent.AdoptiveMother?.Id != Guid.Empty)
                // {
                //     PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.AdoptiveMother.Id);
                //     selectedperson.NationalId = adoptionEvent.AdoptiveMother?.NationalId;
                //     selectedperson.NationalityLookupId = adoptionEvent.AdoptiveMother?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = adoptionEvent.AdoptiveMother?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = adoptionEvent.AdoptiveMother?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = adoptionEvent.AdoptiveMother?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = adoptionEvent.AdoptiveMother?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = adoptionEvent.AdoptiveMother?.NationLookupId;
                //     adoptionEvent.AdoptiveMother = selectedperson;
                // }
                // if (adoptionEvent.Event.EventOwener?.Id != null && adoptionEvent.Event.EventOwener?.Id != Guid.Empty)
                // {
                //     PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.Event.EventOwener.Id);
                //     selectedperson.NationalId = adoptionEvent.Event?.EventOwener?.NationalId;
                //     selectedperson.NationalityLookupId = adoptionEvent.Event?.EventOwener?.NationalityLookupId;
                //     selectedperson.ReligionLookupId = adoptionEvent.Event?.EventOwener?.ReligionLookupId;
                //     selectedperson.EducationalStatusLookupId = adoptionEvent.Event?.EventOwener?.EducationalStatusLookupId;
                //     selectedperson.TypeOfWorkLookupId = adoptionEvent.Event?.EventOwener?.TypeOfWorkLookupId;
                //     selectedperson.MarriageStatusLookupId = adoptionEvent.Event?.EventOwener?.MarriageStatusLookupId;
                //     selectedperson.NationLookupId = adoptionEvent.Event?.EventOwener?.NationLookupId;
                //     adoptionEvent.Event.EventOwener = selectedperson;
                // }
                // _adoptionEventRepository.EFUpdate(adoptionEvent);
                // if (!request.IsFromCommand)
                // {
                //     await _adoptionEventRepository.SaveChangesAsync(cancellationToken);

                // }
                var personIds = new PersonIdObj();

                if (!request.IsFromCommand)
                {
                    adoptionEvent.Event.EventSupportingDocuments = null;
                    if (adoptionEvent.Event.PaymentExamption != null)
                    {
                        adoptionEvent.Event.PaymentExamption.SupportingDocuments = null;
                    }
                    _adoptionEventRepository.EFUpdate(adoptionEvent);
                    personIds = new PersonIdObj
                    {
                        MotherId = adoptionEvent.AdoptiveMother != null ? adoptionEvent.AdoptiveMother.Id : adoptionEvent.AdoptiveMotherId,
                        FatherId = adoptionEvent.AdoptiveFather != null ? adoptionEvent.AdoptiveFather.Id : adoptionEvent.AdoptiveFatherId,
                        ChildId = adoptionEvent.Event.EventOwener != null ? adoptionEvent.Event.EventOwener.Id : adoptionEvent.Event.EventOwenerId
                    };
                    var docs = await _eventDocumentService.createSupportingDocumentsAsync(supportingDocs, examptionsupportingDocs, adoptionEvent.EventId, adoptionEvent.Event.PaymentExamption?.Id, cancellationToken);
                    var result = await _adoptionEventRepository.SaveChangesAsync(cancellationToken);
                    var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, docs.supportingDocs);
                    _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                    _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, (ICollection<SupportingDocument>)docs.examptionDocs, "Birth");

                }
                else
                {
                    _adoptionEventRepository.EFUpdate(adoptionEvent);
                    personIds = new PersonIdObj
                    {
                        MotherId = adoptionEvent.AdoptiveMother != null ? adoptionEvent.AdoptiveMother.Id : adoptionEvent.AdoptiveMotherId,
                        FatherId = adoptionEvent.AdoptiveFather != null ? adoptionEvent.AdoptiveFather.Id : adoptionEvent.AdoptiveFatherId,
                        ChildId = adoptionEvent.Event.EventOwener != null ? adoptionEvent.Event.EventOwener.Id : adoptionEvent.Event.EventOwenerId
                    };
                    var separatedDocs = _eventDocumentService.ExtractOldSupportingDocs(personIds, adoptionEvent.Event.EventSupportingDocuments);
                    _eventDocumentService.MovePhotos(separatedDocs.userPhotos, "Birth");
                    if (!adoptionEvent.Event.IsExampted)
                    {
                        _eventDocumentService.MoveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, adoptionEvent?.Event?.PaymentExamption?.SupportingDocuments, "Birth");
                    }
                }
                // _eventDocumentService.saveSupportingDocuments(adoptionEvent.Event.EventSupportingDocuments, adoptionEvent.Event.PaymentExamption.SupportingDocuments, "Adoption");
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
        // }
        return UpdateAdoptionCommandResponse;
    }
}