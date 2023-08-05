﻿
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Features.Marriage.MarriageEvents.Commands;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create
{

    public class CreateMarriageEventCommandHandler : IRequestHandler<CreateMarriageEventCommand, CreateMarriageEventCommandResponse>
    {
        private readonly IMarriageEventRepository _marriageEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMarriageApplicationRepository _marriageApplicationRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IDivorceEventRepository _divorceEventRepository;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly IPaymentExamptionRequestRepository _paymentExamptionRequestRepository;
        private readonly ISmsService _smsService;
        private readonly ISettingRepository _settingRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<CreateMarriageEventCommandHandler> logger;
        private readonly IAddressLookupRepository _addressRepostory;

        public CreateMarriageEventCommandHandler(IMarriageEventRepository marriageEventRepository,
                                                 IPersonalInfoRepository personalInfoRepository,
                                                 IEventDocumentService eventDocumentService,
                                                 IMarriageApplicationRepository marriageApplicationRepository,
                                                 ILookupRepository lookupRepository,
                                                 IDivorceEventRepository divorceEventRepository,
                                                 IEventPaymentRequestService paymentRequestService,
                                                 IAddressLookupRepository addressRepository,
                                                 IPaymentExamptionRequestRepository paymentExamptionRequestRepository,
                                                 ISmsService smsService,
                                                 ISettingRepository settingRepository,
                                                 IEventRepository eventRepository,
                                                 ILogger<CreateMarriageEventCommandHandler> logger,
                                                 IAddressLookupRepository addressRepostory)
        {
            _marriageEventRepository = marriageEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
            _marriageApplicationRepository = marriageApplicationRepository;
            _lookupRepository = lookupRepository;
            _divorceEventRepository = divorceEventRepository;
            _paymentRequestService = paymentRequestService;
            _addressRepository = addressRepository;
            _paymentExamptionRequestRepository = paymentExamptionRequestRepository;
            _smsService = smsService;
            _settingRepository = settingRepository;
            _eventRepository = eventRepository;
            this.logger = logger;
            _addressRepostory = addressRepostory;
        }

        public async Task<CreateMarriageEventCommandResponse> Handle(CreateMarriageEventCommand request, CancellationToken cancellationToken)
        {
            float amount = 0;
            var executionStrategy = _marriageEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _marriageEventRepository.Database.BeginTransaction())
                {

                    try

                    {
                        var CreateMarriageEventCommandResponse = new CreateMarriageEventCommandResponse();

                        var validator = new CreateMarriageEventCommandValidator(_lookupRepository, _marriageApplicationRepository, _personalInfoRepository, _divorceEventRepository, _marriageEventRepository, _paymentExamptionRequestRepository, _addressRepository, _settingRepository, _eventRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        // Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            CreateMarriageEventCommandResponse.Success = false;
                            CreateMarriageEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                CreateMarriageEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            CreateMarriageEventCommandResponse.Message = CreateMarriageEventCommandResponse.ValidationErrors[0];
                            CreateMarriageEventCommandResponse.Status = 400;
                        }
                        if (CreateMarriageEventCommandResponse.Success)
                        {

                            var marriageEvent = CustomMapper.Mapper.Map<MarriageEvent>(request);
                            if (request?.Event?.EventRegisteredAddressId != null && request?.Event?.EventRegisteredAddressId != Guid.Empty)
                            {
                                var address = await _addressRepostory.GetAsync(request.Event.EventRegisteredAddressId);
                                if (address != null && address.AdminLevel != 5)
                                {
                                    marriageEvent.Event.IsCertified = true;
                                    marriageEvent.Event.IsPaid = true;
                                    marriageEvent.Event.IsOfflineReg = true;
                                }
                                marriageEvent.Event.EventRegisteredAddressId = request?.Event.EventRegisteredAddressId;
                            }
                            var brideHasDivorce = request.BrideInfo.Id != null
                                                && MarriageValidatorFunctions.brideHasDivorceInLessThanDateLimitInSetting((Guid)request.BrideInfo.Id, marriageEvent.Event.EventDateEt, _settingRepository, _personalInfoRepository);
                            var pregnancyFreeSupportingDocAttached = request.Event.EventSupportingDocuments != null
                                                                    && MarriageValidatorFunctions.hasSupportingDoc(request.Event.EventSupportingDocuments, _lookupRepository, "pregnancy free certificate");
                            var hasUnderAgeApprovalSupportingDoc = request.Event.EventSupportingDocuments != null
                                                                    && MarriageValidatorFunctions.hasSupportingDoc(request.Event.EventSupportingDocuments, _lookupRepository, "underage marriage approval");
                            var brideBelowAgeLimit = !MarriageValidatorFunctions.IsAboveTheAgeLimit(request.BrideInfo.BirthDateEt, request.Event.EventDateEt, true, _settingRepository);
                            var groomBelowAgeLimit = !MarriageValidatorFunctions.IsAboveTheAgeLimit(request.Event.EventOwener.BirthDateEt, request.Event.EventDateEt, false, _settingRepository);
                            if ((brideHasDivorce && pregnancyFreeSupportingDocAttached) || ((brideBelowAgeLimit || groomBelowAgeLimit) && hasUnderAgeApprovalSupportingDoc))
                            {
                                marriageEvent.Event.HasPendingDocumentApproval = true;
                            }
                            marriageEvent.Event.EventType = "Marriage";
                            await _marriageEventRepository.InsertOrUpdateAsync(marriageEvent, cancellationToken);

                            // //TODO: //
                            var personIds = new PersonIdObj
                            {
                                WifeId = marriageEvent.BrideInfo != null ? marriageEvent.BrideInfo.Id : marriageEvent.BrideInfoId,
                                HusbandId = marriageEvent.Event.EventOwener != null ? marriageEvent.Event.EventOwener.Id : marriageEvent.Event.EventOwenerId,
                                WitnessIds = marriageEvent.Witnesses
                                        .Select(w => w.WitnessPersonalInfo != null ? w.WitnessPersonalInfo.Id : w.WitnessPersonalInfoId).ToList()
                            };
                            var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, marriageEvent.Event.EventSupportingDocuments);
                            _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                            _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, marriageEvent.Event.PaymentExamption?.SupportingDocuments, "Marriage");
                            _eventDocumentService.saveFingerPrints(separatedDocs.fingerPrint);

                            // create payment request for the event if it is not exempted
                            if (!marriageEvent.Event.IsExampted)
                            {
                                (float amount, string code) response = await _paymentRequestService.CreatePaymentRequest("Marriage", marriageEvent.Event, "CertificateGeneration", null, marriageEvent.HasCamera, marriageEvent.HasVideo, cancellationToken);
                                amount = response.amount;
                                if (response.amount == 0)
                                {
                                    marriageEvent.Event.IsPaid = true;
                                }
                                else
                                {
                                    string message = $"Dear Customer,\nThis is to inform you that your request for Marriage certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {response.amount} ETB to finance office using code {response.code}.\n OCRA";
                                    List<string> msgRecepients = new List<string>();
                                    if (marriageEvent.BrideInfo?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(marriageEvent.BrideInfo.PhoneNumber);
                                    }
                                    if (marriageEvent.Event.EventOwener?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(marriageEvent.Event.EventOwener.PhoneNumber);
                                    }
                                    await _smsService.SendBulkSMS(msgRecepients, message);
                                }
                            }
                            // else if (amount != 0 || marriageEvent.Event.IsExampted)
                            // {
                            await _marriageEventRepository.SaveChangesAsync(cancellationToken);

                            CreateMarriageEventCommandResponse.Message = "Marriage Event created Successfully";
                            // }
                            await transaction.CommitAsync();

                        }
                        return CreateMarriageEventCommandResponse;
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }

    }
}
