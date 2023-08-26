using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;
using System.Text.Json;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{
    // create death event command handler
    public class CreateDeathEventCommandHandler : IRequestHandler<CreateDeathEventCommand, CreateDeathEventCommandResponse>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly ISmsService _smsService;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly IAddressLookupRepository _addressRepostory;
        private readonly IFingerprintService _fingerprintService;
        private readonly IUserResolverService _userResolverService;
        public CreateDeathEventCommandHandler(IDeathEventRepository deathEventRepository,
                                              IEventRepository eventRepository,
                                              IEventDocumentService eventDocumentService,
                                              ISmsService smsService,
                                              IEventPaymentRequestService paymentRequestService,
                                              IAddressLookupRepository addressRepostory,
                                              IFingerprintService fingerprintService,
                                              IUserResolverService userResolverService)

        {
            _deathEventRepository = deathEventRepository;
            _eventRepository = eventRepository;
            _eventDocumentService = eventDocumentService;
            _smsService = smsService;
            _paymentRequestService = paymentRequestService;
            _addressRepostory = addressRepostory;
            _fingerprintService=fingerprintService;
            _userResolverService=userResolverService;
        }
        public async Task<CreateDeathEventCommandResponse> Handle(CreateDeathEventCommand request, CancellationToken cancellationToken)
        {
            // Payment amount for death certificate
            float amount = 0;
            var executionStrategy = _deathEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using var transaction = _deathEventRepository.Database.BeginTransaction();
                try
                {
                    var response = new CreateDeathEventCommandResponse();

                    var validator = new CreateDeathEventCommandValidator(_eventRepository);
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
                    if (response.Success)
                    {
                        try
                        {
                            // Map the request to the model entity.
                            var deathEvent = CustomMapper.Mapper.Map<DeathEvent>(request.DeathEvent);
                            if (request.DeathEvent?.Event?.EventRegisteredAddressId != null && request.DeathEvent?.Event?.EventRegisteredAddressId != Guid.Empty)
                            {
                                 var address = await _addressRepostory.GetAsync(_userResolverService.GetWorkingAddressId());
                                if(address==null){
                                        throw new NotFoundException("Invalid user working address");
                                    }
                                if (address != null && address.AdminLevel != 5)
                                {
                                    deathEvent.Event.IsCertified = true;
                                    deathEvent.Event.IsPaid = true;
                                    deathEvent.Event.IsOfflineReg = true;
                                }
                                deathEvent.Event.EventRegisteredAddressId = request.DeathEvent?.Event.EventRegisteredAddressId;
                            }
                            await _deathEventRepository.InsertOrUpdateAsync(deathEvent, cancellationToken);
                            // Persons id
                            var personIds = new PersonIdObj
                            {
                                DeceasedId = deathEvent.Event.EventOwener != null ? deathEvent.Event.EventOwener.Id : deathEvent.Event.EventOwenerId,
                                RegistrarId = deathEvent.Event.EventRegistrar?.RegistrarInfo != null ? deathEvent.Event.EventRegistrar?.RegistrarInfo.Id : deathEvent.Event.EventRegistrar?.RegistrarInfoId
                            };

                            // Save the supporting documents and payment exemption documents.
                            var (userPhotos, fingerprints, otherDocs) = _eventDocumentService.extractSupportingDocs(personIds, deathEvent.Event.EventSupportingDocuments);
                            _eventDocumentService.savePhotos(userPhotos);
                            //  var FingerPrintResponse= await _fingerprintService.RegisterfingerPrintService(fingerprints,cancellationToken);
                            //     if(!FingerPrintResponse.Success){ 
                            //         response.Message="Duplicated Fingerprint";
                            //         response.Success=false; 
                            //         return response;
                            //         }
                            _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)otherDocs, deathEvent.Event.PaymentExamption?.SupportingDocuments, "Death");
                            _eventDocumentService.saveFingerPrints(fingerprints);
                            if (!deathEvent.Event.IsExampted)
                            {
                                // Get Payment rate for death certificate.
                                (float amount, string code) payment = await _paymentRequestService.CreatePaymentRequest("Death", deathEvent.Event, "CertificateGeneration", null, false, false, cancellationToken);
                                amount = payment.amount;
                                if (payment.amount == 0)
                                {
                                    deathEvent.Event.IsPaid = true;
                                }
                                else
                                {
                                    string message = $"Dear Customer,\nThis is to inform you that your request for Death certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {payment.amount} ETB to finance office using code {payment.code}.\n OCRA";
                                    if (deathEvent.Event.EventRegistrar?.RegistrarInfo.PhoneNumber != null)
                                    {
                                        await _smsService.SendSMS(deathEvent.Event.EventRegistrar.RegistrarInfo.PhoneNumber, message);
                                    }
                                }
                            }
                            // Insert into the database.
                            // var result = await _deathEventRepository.SaveChangesAsync(cancellationToken);
                        }
                        catch (System.Exception ex)
                        {
                            response.BadRequest(ex.Message);
                            throw;
                        }

                        // Set the response to created.
                        response.Created("Death Event");
                        // Commit the transaction.
                        await transaction.CommitAsync();
                        _deathEventRepository.TriggerPersonalInfoIndex();
                    }
                    return response;
                }
                catch (Exception)
                {
                    // Roleback the transaction on exception.
                    await transaction.RollbackAsync();
                    throw;
                }

            });
        }
    }
}
