﻿
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Service;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create
{

    public class CreateDivorceEventCommandHandler : IRequestHandler<CreateDivorceEventCommand, CreateDivorceEventCommandResponse>
    {
        private readonly IDivorceEventRepository _DivorceEventRepository;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly ILookupRepository _lookupRepository;
        private readonly IAddressLookupRepository _addressLookupRepository;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly ISmsService _smsService;
        private readonly IEventRepository _eventRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly IAddressLookupRepository _addressRepostory;
        private readonly IFingerprintService _fingerprintService;
        private readonly IUserResolverService _userResolverService;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventStatusService _eventStatusService;


        public CreateDivorceEventCommandHandler(IDivorceEventRepository DivorceEventRepository,
                                                IPersonalInfoRepository personalInfoRepository,
                                                IEventDocumentService eventDocumentService,
                                                ILookupRepository lookupRepository,
                                                IAddressLookupRepository addressLookupRepository,
                                                IEventPaymentRequestService paymentRequestService,
                                                ISmsService smsService,
                                                IEventRepository eventRepository,
                                                ICourtRepository courtRepository,
                                                IAddressLookupRepository addressRepostory,
                                                IFingerprintService fingerprintService,
                                                IUserResolverService userResolverService,
                                                ICertificateRepository certificateRepository,
                                                IEventStatusService eventStatusService
                                                )
        {
            _DivorceEventRepository = DivorceEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
            _lookupRepository = lookupRepository;
            _addressLookupRepository = addressLookupRepository;
            _paymentRequestService = paymentRequestService;
            _smsService = smsService;
            _eventRepository = eventRepository;
            _courtRepository = courtRepository;
            _addressRepostory = addressRepostory;
            _fingerprintService = fingerprintService;
            _userResolverService = userResolverService;
            _certificateRepository = certificateRepository;
            _eventStatusService = eventStatusService;
        }
        public async Task<CreateDivorceEventCommandResponse> Handle(CreateDivorceEventCommand request, CancellationToken cancellationToken)
        {
            float amount = 0;
            bool IsManualRegistration = false;
            var executionStrategy = _DivorceEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = request.IsFromBgService ? null : _DivorceEventRepository.Database.BeginTransaction())
                {
                    try
                    {
                        var createDivorceEventCommandResponse = new CreateDivorceEventCommandResponse();

                        var validator = new CreateDivorceEventCommandValidator(_personalInfoRepository, _lookupRepository, _addressLookupRepository, _courtRepository, _eventRepository);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            createDivorceEventCommandResponse.Success = false;
                            createDivorceEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                createDivorceEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            createDivorceEventCommandResponse.Message = createDivorceEventCommandResponse.ValidationErrors[0];
                            createDivorceEventCommandResponse.Status = 400;
                        }
                        if (createDivorceEventCommandResponse.Success)
                        {
                            Guid workingAddressId = await HelperService.GetWorkingAddressId(_userResolverService, _personalInfoRepository, request.IsFromBgService ? request.Event.CivilRegOfficerId : null);
                            var address = await _addressRepostory.GetAsync(workingAddressId);
                            request.Event.EventDateEt = request?.CourtCase?.ConfirmedDateEt!;
                            // request.Event.EventAddressId = request?.CourtCase?.Court?.AddressId!;
                            if (request?.CourtCase?.CourtId != null && request?.CourtCase?.CourtId != Guid.Empty)
                            {
                                request.CourtCase.Court = null;
                            }
                            var divorceEvent = CustomMapper.Mapper.Map<DivorceEvent>(request);
                            divorceEvent.Event.Status = _eventStatusService.ReturnEventStatus("birth", divorceEvent.Event.EventDate, divorceEvent.Event.EventRegDate);

                            if (request?.Event?.EventRegisteredAddressId != null && request?.Event?.EventRegisteredAddressId != Guid.Empty)
                            {
                                if (address == null)
                                {
                                    throw new NotFoundException("Invalid user working address");
                                }
                                if (address != null && address.AdminLevel != 5)
                                {
                                    divorceEvent.Event.IsCertified = true;
                                    divorceEvent.Event.IsPaid = true;
                                    divorceEvent.Event.IsOfflineReg = true;
                                    divorceEvent.Event.ReprintWaiting = false;
                                    IsManualRegistration = true;
                                }
                                divorceEvent.Event.EventRegisteredAddressId = request?.Event?.EventRegisteredAddressId;
                            }
                            divorceEvent.Event.EventType = "Divorce";
                            await _DivorceEventRepository.InsertOrUpdateAsync(divorceEvent, cancellationToken);

                            var personIds = new PersonIdObj
                            {
                                WifeId = divorceEvent.DivorcedWife != null ? divorceEvent.DivorcedWife?.Id : divorceEvent.DivorcedWifeId,
                                HusbandId = divorceEvent.Event.EventOwener != null ? divorceEvent.Event?.EventOwener?.Id : divorceEvent.Event.EventOwenerId,
                            };
                            // await _DivorceEventRepository.SaveChangesAsync(cancellationToken);

                            var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, divorceEvent.Event.EventSupportingDocuments);
                            _eventDocumentService.savePhotos(separatedDocs.UserPhoto);
                            _eventDocumentService.savePhotos(separatedDocs.Signatures, "Signatures");

                            _eventDocumentService.saveSupportingDocuments(separatedDocs.OtherDocs, divorceEvent.Event.PaymentExamption?.SupportingDocuments, "Divorce");
                            _eventDocumentService.saveFingerPrints(separatedDocs.FingerPrints);
                            //    var FingerPrintResponse= await _fingerprintService.RegisterfingerPrintService(separatedDocs.fingerPrint,cancellationToken);
                            //     if(!FingerPrintResponse.Success){ 
                            //         createDivorceEventCommandResponse.Message ="Duplicated Fingerprint";
                            //         createDivorceEventCommandResponse.Success=false; 
                            //         return createDivorceEventCommandResponse;
                            //         }
                            if ((!divorceEvent.Event.IsExampted) && (address != null && address?.AdminLevel == 5))
                            {
                                (float amount, string code) response = await _paymentRequestService.CreatePaymentRequest("Divorce", divorceEvent.Event, "CertificateGeneration", null, false, false, cancellationToken);
                                amount = response.amount;
                                if (response.amount == 0)
                                {
                                    divorceEvent.Event.IsPaid = true;
                                }
                                else
                                {
                                    string message = $"Dear Customer,\nThis is to inform you that your request for Divorce certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {response.amount} ETB to finance office.\n OCRA";
                                    List<string> msgRecepients = new();
                                    if (divorceEvent.DivorcedWife?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(divorceEvent.DivorcedWife.PhoneNumber);
                                    }
                                    if (divorceEvent.Event.EventOwener?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(divorceEvent.Event.EventOwener.PhoneNumber);
                                    }
                                    await _smsService.SendBulkSMS(msgRecepients, message);

                                }
                            }
                            await _DivorceEventRepository.SaveChangesAsync(cancellationToken);
                            // if (amount != 0 || divorceEvent.Event.IsExampted)
                            // {
                            createDivorceEventCommandResponse.Message = "Divorce event created successfully";
                            createDivorceEventCommandResponse.IsManualRegistration = IsManualRegistration;
                            createDivorceEventCommandResponse.EventId = divorceEvent.Event.Id;
                            createDivorceEventCommandResponse.divorceEventRepository = request.IsFromBgService ? _DivorceEventRepository:null;


                            if (transaction != null)
                            {
                                await transaction.CommitAsync();
                                _DivorceEventRepository.TriggerPersonalInfoIndex();
                            }
                            // _certificateRepository.TriggerCertificateIndex();

                            // }
                        }
                        return createDivorceEventCommandResponse;
                    }
                    catch (System.Exception)
                    {
                        if (transaction != null)
                        {
                            await transaction.RollbackAsync();
                        }
                        throw;
                    }
                }
            });


        }
    }
}
