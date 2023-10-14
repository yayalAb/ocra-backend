﻿using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Birth event command handler
    public class CreateBirthEventCommandHandler : IRequestHandler<CreateBirthEventCommand, CreateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly ILookupRepository _lookupRepository;
        private readonly ISmsService _smsService;
        private readonly IAddressLookupRepository _addressRepostory;
        private readonly IFingerprintService _fingerprintService;
        private readonly IUserResolverService _userResolverService;
        private readonly IPersonalInfoRepository _personalInfoRepository;
        private readonly IEventStatusService _eventStatusService;


        public CreateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventRepository eventRepository,
                                              IEventDocumentService eventDocumentService,
                                              IEventPaymentRequestService paymentRequestService,
                                              ILookupRepository lookupRepository,
                                              ISmsService smsService,
                                              IAddressLookupRepository addressRepostory,
                                              IFingerprintService fingerprintService,
                                              IUserResolverService userResolverService,
                                              IPersonalInfoRepository personalInfoRepository,
                                              IEventStatusService eventStatusService
                                              )
        {
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            _birthEventRepository = birthEventRepository;
            _paymentRequestService = paymentRequestService;
            _lookupRepository = lookupRepository;
            _smsService = smsService;
            _addressRepostory = addressRepostory;
            _fingerprintService = fingerprintService;
            _userResolverService = userResolverService;
            _personalInfoRepository = personalInfoRepository;
            _eventStatusService = eventStatusService;
        }
        public async Task<CreateBirthEventCommandResponse> Handle(CreateBirthEventCommand request, CancellationToken cancellationToken)
        {
            // payment amount for birth event.
            float amount = 0;
            bool IsManualRegistration = false;
            // Create an execution strategy for the current database.
            var executionStrategy = _birthEventRepository.Database.CreateExecutionStrategy();
            List<PersonalInfoEntry> personalInfoEntries = new List<PersonalInfoEntry>();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                // Begin new transaction if not from background service ...... since transaction is defined in the bg service
                using var transaction = request.BirthEvent.IsFromBgService ? null : _birthEventRepository.Database.BeginTransaction();

                try
                {
                    // Create new response for birth event.
                    var response = new CreateBirthEventCommandResponse();
                    // Validate the inputs.
                    var validator = new CreateBirthEventCommandValidator(_eventRepository, _lookupRepository);
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);

                    //Check and log validation errors
                    if (validationResult.Errors.Count > 0)
                    {
                        response.Success = false;
                        response.Status = 400;
                        response.ValidationErrors = new List<string>();
                        foreach (var error in validationResult.Errors)
                            response.ValidationErrors.Add(error.ErrorMessage);
                        response.Message = response.ValidationErrors[0];
                    }
                    else if (response.Success)
                    {
                        try
                        {
                            Guid workingAddressId = await HelperService.GetWorkingAddressId(_userResolverService, _personalInfoRepository, request.BirthEvent.IsFromBgService ? request.BirthEvent.Event.CivilRegOfficerId : null);
                            var address = await _addressRepostory.GetAsync(workingAddressId);
                            // Map the request to the model entity.
                            var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request.BirthEvent);
                            birthEvent.Event.Status = _eventStatusService.ReturnEventStatus("birth", birthEvent.Event.EventDate, birthEvent.Event.EventRegDate);
                            if (request.BirthEvent?.Event?.EventRegisteredAddressId != null && request.BirthEvent?.Event?.EventRegisteredAddressId != Guid.Empty)
                            {
                                if (address == null)
                                {
                                    throw new NotFoundException("Invalid user working address");
                                }
                                if (address != null && address.AdminLevel != 5)
                                {
                                    birthEvent.Event.IsCertified = true;
                                    birthEvent.Event.IsPaid = true;
                                    birthEvent.Event.IsOfflineReg = true;
                                    birthEvent.Event.ReprintWaiting = false;
                                    IsManualRegistration = true;
                                }
                                birthEvent.Event.EventRegisteredAddressId = request.BirthEvent?.Event.EventRegisteredAddressId;
                            }
                            if (request.BirthEvent.Event.InformantType == "guardian")
                            {
                                birthEvent.Event.HasPendingDocumentApproval = true;
                            }
                            // Insert to the database.
                            await _birthEventRepository.InsertOrUpdateAsync(birthEvent, cancellationToken);
                            // store the persons id from the request
                            var personIds = new PersonIdObj
                            {
                                MotherId = birthEvent.Mother != null ? birthEvent.Mother?.Id : birthEvent.MotherId,
                                FatherId = birthEvent.Father != null ? birthEvent.Father?.Id : birthEvent.FatherId,
                                ChildId = birthEvent.Event.EventOwener != null ? birthEvent.Event.EventOwener.Id : birthEvent.Event.EventOwenerId,
                                RegistrarId = birthEvent.Event.EventRegistrar?.RegistrarInfo != null ? birthEvent.Event.EventRegistrar?.RegistrarInfo.Id : birthEvent.Event.EventRegistrar?.RegistrarInfoId
                            };
                            // Separate profile photos from supporting documents.
                            var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, birthEvent.Event.EventSupportingDocuments);
                            // var FingerPrintResponse= await _fingerprintService.RegisterfingerPrintService(fingerprints,cancellationToken);
                            // if(!FingerPrintResponse.Success){ 
                            //       response.Message="Duplicated Fingerprint";
                            //       response.Success=false; 
                            //       return response;
                            //     }
                            _eventDocumentService.savePhotos(separatedDocs.UserPhoto);
                            _eventDocumentService.savePhotos(separatedDocs.Signatures, "Signatures");
                            _eventDocumentService.saveSupportingDocuments(separatedDocs.OtherDocs, birthEvent.Event.PaymentExamption?.SupportingDocuments, "Birth");
                            _eventDocumentService.saveFingerPrints(separatedDocs.FingerPrints);

                            // For non exempted documents 
                            if ((!birthEvent.Event.IsExampted) && (address != null && address?.AdminLevel == 5))
                            {
                                (float amount, string code) payment = await _paymentRequestService.CreatePaymentRequest("Birth", birthEvent.Event, "CertificateGeneration", null, false, false, cancellationToken);
                                if (payment.amount == 0)
                                {
                                    birthEvent.Event.IsPaid = true;
                                }
                                else
                                {
                                    string message = $"Dear Customer,\nThis is to inform you that your request for Birth certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {payment.amount} ETB to finance office using code {payment.code}.\n OCRA";
                                    List<string> msgRecepients = new();
                                    if (birthEvent?.Mother?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(birthEvent?.Mother?.PhoneNumber!);
                                    }
                                    if (birthEvent?.Father?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(birthEvent?.Father?.PhoneNumber!);
                                    }
                                    if (birthEvent?.Event.EventRegistrar?.RegistrarInfo?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(birthEvent.Event.EventRegistrar.RegistrarInfo.PhoneNumber);
                                    }
                                    await _smsService.SendBulkSMS(msgRecepients, message);
                                }
                                // Save Changes. 
                                await _birthEventRepository.SaveChangesAsync(cancellationToken);
                                response.Message = "Birth Event created Successfully";
                                response.Status = 200;
                                response.IsManualRegistration = IsManualRegistration;
                                response.EventId = birthEvent.Event.Id;
                                response.birthEventRepository = request.BirthEvent.IsFromBgService ? _birthEventRepository: null;
                                if (transaction != null)
                                {
                                    await transaction.CommitAsync();
                                    _birthEventRepository.TriggerPersonalInfoIndex();
                                }

                                // }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            response.Success = false;
                            response.Status = 400;
                            throw;
                        }

                    }

                    return response;

                }
                catch (Exception)
                {
                    // Rollback the transaction on exception.
                    if (transaction != null)
                    {
                        await transaction.RollbackAsync();
                    }
                    throw;
                }

            });
        }

    }
}
