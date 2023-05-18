using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public class CreateAdoptionCommandHandler : IRequestHandler<CreateAdoptionCommand, CreateAdoptionCommandResponse>
    {
        private readonly IAdoptionEventRepository _AdoptionEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly IFileService _fileService;
        private readonly IEventDocumentService _eventDocumentService;
        public CreateAdoptionCommandHandler(IAddressLookupRepository addressRepository, ICourtRepository courtQueryRepository, IEventDocumentService eventDocumentService, IAdoptionEventRepository AdoptionEventRepository, IPersonalInfoRepository personalInfoRepository, IFileService fileService)
        {
            _AdoptionEventRepository = AdoptionEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _courtRepository = courtQueryRepository;
            _fileService = fileService;
            _eventDocumentService = eventDocumentService;
            _addressRepository = addressRepository;
        }
        public async Task<CreateAdoptionCommandResponse> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
        {
            var CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse();

            var validator = new CreatAdoptionCommandValidator(_AdoptionEventRepository, _addressRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                CreateAdoptionCommandResponse.Success = false;
                CreateAdoptionCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateAdoptionCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateAdoptionCommandResponse.Message = CreateAdoptionCommandResponse.ValidationErrors[0];
            }
            else if (CreateAdoptionCommandResponse.Success)
            {
                try
                {
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

                        _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(selectedperson));
                        adoptionEvent.AdoptiveFatherId = adoptionEvent.AdoptiveFather.Id;
                        adoptionEvent.AdoptiveFather = null;
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

                        _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(selectedperson));
                        adoptionEvent.AdoptiveMotherId = adoptionEvent.AdoptiveMother.Id;
                        adoptionEvent.AdoptiveMother = null;
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

                        _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(selectedperson));
                        adoptionEvent.Event.EventOwenerId = adoptionEvent.Event.EventOwener.Id;
                        adoptionEvent.Event.EventOwener = null;
                    }
                    if (adoptionEvent.CourtCase?.Court?.Id != null && adoptionEvent.CourtCase?.Court?.Id != Guid.Empty)
                    {
                        _courtRepository.Update(CustomMapper.Mapper.Map<Court>(adoptionEvent.CourtCase.Court));
                        adoptionEvent.CourtCase.CourtId = adoptionEvent.CourtCase.Court.Id;
                        adoptionEvent.CourtCase.Court = null;
                    }
                    await _AdoptionEventRepository.InsertAsync(adoptionEvent, cancellationToken);
                    await _AdoptionEventRepository.SaveChangesAsync(cancellationToken);
                    _eventDocumentService.saveSupportingDocuments(adoptionEvent?.Event?.EventSupportingDocuments, adoptionEvent?.Event?.PaymentExamption?.SupportingDocuments, "Adoption");
                    CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse
                    {
                        Success = true,
                        Message = "Adoption Event created Successfully"
                    };
                }
                catch (Exception ex)
                {

                    CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse
                    {
                        Status = 500,
                        Success = false,
                        Message = ex.Message
                    };
                    throw;

                }
            }
            return CreateAdoptionCommandResponse;

        }
    }
}




