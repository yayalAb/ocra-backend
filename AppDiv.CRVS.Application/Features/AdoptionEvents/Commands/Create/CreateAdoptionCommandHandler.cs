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
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;

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
        private readonly IPersonalInfoRepository _PersonalInfo;
        private readonly ILookupRepository _LookupsRepo;
        private readonly IPaymentExamptionRequestRepository _PaymentExaptionRepo;
        private readonly IEventRepository _EventRepository;
        private readonly IEventPaymentRequestService _paymentRequestService;

        private readonly ISmsService _smsService;

        public CreateAdoptionCommandHandler(
                                        IPersonalInfoRepository PersonalInfo,
                                        IAddressLookupRepository addressRepository,
                                        ICourtRepository courtQueryRepository,
                                        IEventDocumentService eventDocumentService,
                                        IAdoptionEventRepository AdoptionEventRepository,
                                        IPersonalInfoRepository personalInfoRepository,
                                        IFileService fileService, ILookupRepository LookupsRepo,
                                        IPaymentExamptionRequestRepository PaymentExaptionRepo,
                                        IEventRepository EventRepository,
                                        IEventPaymentRequestService paymentRequestService,
                                        ISmsService smsService)
        {
            _AdoptionEventRepository = AdoptionEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _courtRepository = courtQueryRepository;
            _fileService = fileService;
            _eventDocumentService = eventDocumentService;
            _addressRepository = addressRepository;
            _PersonalInfo = PersonalInfo;
            _LookupsRepo = LookupsRepo;
            _PaymentExaptionRepo = PaymentExaptionRepo;
            _EventRepository = EventRepository;
            _paymentRequestService = paymentRequestService;
            _smsService = smsService;
        }
        public async Task<CreateAdoptionCommandResponse> Handle(CreateAdoptionCommand request, CancellationToken cancellationToken)
        {
            float amount = 0;
            var executionStrategy = _AdoptionEventRepository.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = _AdoptionEventRepository.Database.BeginTransaction())
                {
                    try
                    {
                        request.Adoption.Event.EventOwener.MiddleName = request?.Adoption?.AdoptiveFather?.FirstName;
                        request.Adoption.Event.EventOwener.LastName = request?.Adoption?.AdoptiveFather?.MiddleName;
                        var CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse();
                        var validator = new CreatAdoptionCommandValidator(_AdoptionEventRepository,
                                                                        _addressRepository, _PersonalInfo, _LookupsRepo,
                                                                        _PaymentExaptionRepo, _EventRepository);
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
                                request.Adoption.AdoptiveFather.SexLookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                                    .Where(l => EF.Functions.Like(l.ValueStr, "%ወንድ%")
                                                                        || EF.Functions.Like(l.ValueStr, "%Dhiira%")
                                                                        || EF.Functions.Like(l.ValueStr, "%Male%"))
                                                                    .Select(l => l.Id).FirstOrDefault();

                                request.Adoption.AdoptiveMother.SexLookupId = _LookupsRepo.GetAll().Where(l => l.Key == "sex")
                                                                        .Where(l => EF.Functions.Like(l.ValueStr, "%ሴት%")
                                                                            || EF.Functions.Like(l.ValueStr, "%Dubara%")
                                                                            || EF.Functions.Like(l.ValueStr, "%Female%"))
                                                                        .Select(l => l.Id).FirstOrDefault();
                                request.Adoption.Event.EventDateEt = request.Adoption.CourtCase.ConfirmedDateEt;


                                var adoptionEvent = CustomMapper.Mapper.Map<AdoptionEvent>(request.Adoption);

                                if (request?.Adoption?.Event?.EventRegisteredAddressId != null && request?.Adoption?.Event?.EventRegisteredAddressId != Guid.Empty)
                                {
                                    adoptionEvent.Event.EventRegisteredAddressId = request?.Adoption?.Event?.EventRegisteredAddressId;
                                }
                                adoptionEvent.Event.EventAddressId = request?.Adoption?.CourtCase?.Court?.AddressId;

                                if (adoptionEvent.AdoptiveFather?.Id != null && adoptionEvent?.AdoptiveFather?.Id != Guid.Empty)
                                {
                                    PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.AdoptiveFather.Id);
                                    // selectedperson.SexLookupId = adoptionEvent.AdoptiveFather.SexLookupId;
                                    selectedperson.NationalId = adoptionEvent.AdoptiveFather?.NationalId;
                                    selectedperson.NationalityLookupId = adoptionEvent.AdoptiveFather?.NationalityLookupId;
                                    selectedperson.ReligionLookupId = adoptionEvent.AdoptiveFather?.ReligionLookupId;
                                    selectedperson.EducationalStatusLookupId = adoptionEvent.AdoptiveFather?.EducationalStatusLookupId;
                                    selectedperson.TypeOfWorkLookupId = adoptionEvent.AdoptiveFather?.TypeOfWorkLookupId;
                                    selectedperson.MarriageStatusLookupId = adoptionEvent.AdoptiveFather?.MarriageStatusLookupId;
                                    selectedperson.NationLookupId = adoptionEvent.AdoptiveFather?.NationLookupId;
                                    selectedperson.PhoneNumber = adoptionEvent.AdoptiveFather?.PhoneNumber;

                                    _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(selectedperson));
                                    adoptionEvent.AdoptiveFatherId = adoptionEvent.AdoptiveFather.Id;
                                    adoptionEvent.AdoptiveFather = null;
                                }
                                if (adoptionEvent.AdoptiveMother?.Id != null && adoptionEvent.AdoptiveMother?.Id != Guid.Empty)
                                {
                                    PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.AdoptiveMother.Id);
                                    // selectedperson.SexLookupId = adoptionEvent.AdoptiveMother.SexLookupId;
                                    selectedperson.NationalId = adoptionEvent.AdoptiveMother?.NationalId;
                                    selectedperson.NationalityLookupId = (Guid)adoptionEvent.AdoptiveMother?.NationalityLookupId;
                                    selectedperson.ReligionLookupId = adoptionEvent.AdoptiveMother?.ReligionLookupId;
                                    selectedperson.EducationalStatusLookupId = adoptionEvent.AdoptiveMother?.EducationalStatusLookupId;
                                    selectedperson.TypeOfWorkLookupId = adoptionEvent.AdoptiveMother?.TypeOfWorkLookupId;
                                    selectedperson.MarriageStatusLookupId = adoptionEvent.AdoptiveMother?.MarriageStatusLookupId;
                                    selectedperson.NationLookupId = adoptionEvent.AdoptiveMother?.NationLookupId;
                                    selectedperson.PhoneNumber = adoptionEvent.AdoptiveMother?.PhoneNumber;

                                    _personalInfoRepository.EFUpdate(CustomMapper.Mapper.Map<PersonalInfo>(selectedperson));
                                    adoptionEvent.AdoptiveMotherId = adoptionEvent.AdoptiveMother.Id;
                                    adoptionEvent.AdoptiveMother = null;
                                }
                                if (adoptionEvent.Event.EventOwener?.Id != null && adoptionEvent.Event.EventOwener?.Id != Guid.Empty)
                                {
                                    PersonalInfo selectedperson = _personalInfoRepository.GetById(adoptionEvent.Event.EventOwener.Id);
                                    selectedperson.NationalId = adoptionEvent.Event?.EventOwener?.NationalId;
                                    selectedperson.NationalityLookupId = (Guid)adoptionEvent.Event?.EventOwener?.NationalityLookupId;
                                    selectedperson.ReligionLookupId = adoptionEvent.Event?.EventOwener?.ReligionLookupId;
                                    selectedperson.EducationalStatusLookupId = adoptionEvent.Event?.EventOwener?.EducationalStatusLookupId;
                                    selectedperson.TypeOfWorkLookupId = adoptionEvent.Event?.EventOwener?.TypeOfWorkLookupId;
                                    selectedperson.MarriageStatusLookupId = adoptionEvent.Event?.EventOwener?.MarriageStatusLookupId;
                                    selectedperson.NationLookupId = adoptionEvent.Event?.EventOwener?.NationLookupId;
                                    selectedperson.PhoneNumber = adoptionEvent.Event?.EventOwener?.PhoneNumber;

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
                                var personIds = new PersonIdObj
                                {
                                    MotherId = adoptionEvent?.AdoptiveMother?.Id,
                                    FatherId = adoptionEvent?.AdoptiveFather?.Id,
                                    ChildId = adoptionEvent?.Event.EventOwener?.Id
                                };
                                var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, adoptionEvent.Event.EventSupportingDocuments);
                                _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                                _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, adoptionEvent?.Event?.PaymentExamption?.SupportingDocuments, "Adoption");
                                _eventDocumentService.saveFingerPrints(separatedDocs.fingerPrint);

                                if (!adoptionEvent.Event.IsExampted)
                                {
                                    //--create payment request and send sms notification to the users
                                    (float amount, string code) response = await _paymentRequestService.CreatePaymentRequest("Adoption", adoptionEvent.Event, "CertificateGeneration", null, false, false, cancellationToken);
                                    amount = response.amount;
                                    if (response.amount == 0 || response.amount == 0.0)
                                    {
                                        adoptionEvent.Event.IsPaid = true;
                                    }
                                    else
                                    {
                                        string message = $"Dear Customer,\nThis is to inform you that your request for Adoption certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {response.amount} ETB to finance office using  code {response.code}.\n OCRA";
                                        List<string> msgRecepients = new List<string>();
                                        if (adoptionEvent.AdoptiveFather?.PhoneNumber != null)
                                        {
                                            msgRecepients.Add(adoptionEvent.AdoptiveFather.PhoneNumber);
                                        }
                                        if (adoptionEvent.AdoptiveMother?.PhoneNumber != null)
                                        {
                                            msgRecepients.Add(adoptionEvent.AdoptiveMother.PhoneNumber);
                                        }
                                        await _smsService.SendBulkSMS(msgRecepients, message);
                                    }


                                    //

                                }

                                await _AdoptionEventRepository.SaveChangesAsync(cancellationToken);
                                // if (amount != 0 || adoptionEvent.Event.IsExampted)
                                // {
                                await transaction.CommitAsync();
                                CreateAdoptionCommandResponse = new CreateAdoptionCommandResponse
                                {
                                    Success = true,
                                    Message = "Adoption Event created Successfully"
                                };
                                // }

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
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });

        }

    }
}




