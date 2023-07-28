using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Birth event command handler
    public class CreateBirthEventCommandHandler : IRequestHandler<CreateBirthEventCommand, CreateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly ISmsService _smsService;

        public CreateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventRepository eventRepository,
                                              IEventDocumentService eventDocumentService,
                                              IEventPaymentRequestService paymentRequestService,
                                              ISmsService smsService)
        {
            _eventDocumentService = eventDocumentService;
            _eventRepository = eventRepository;
            _birthEventRepository = birthEventRepository;
            _paymentRequestService = paymentRequestService;
            _smsService = smsService;
        }
        public async Task<CreateBirthEventCommandResponse> Handle(CreateBirthEventCommand request, CancellationToken cancellationToken)
        {
            // payment amount for birth event.
            float amount = 0;
            // Create an execution strategy for the current database.
            var executionStrategy = _birthEventRepository.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                // Begin new transaction.
                using var transaction = _birthEventRepository.Database.BeginTransaction();

                try
                {
                    // Create new response for birth event.
                    var response = new CreateBirthEventCommandResponse();
                    // Validate the inputs.
                    var validator = new CreateBirthEventCommandValidator(_eventRepository);
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
                            // Map the request to the model entity.
                            var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request.BirthEvent);
                            // Insert to the database.
                            await _birthEventRepository.InsertOrUpdateAsync(birthEvent, cancellationToken);
                            // store the persons id from the request
                            var personIds = new PersonIdObj
                            {
                                MotherId = birthEvent.Mother?.Id,
                                FatherId = birthEvent.Father?.Id,
                                ChildId = birthEvent.Event.EventOwener.Id,
                                RegistrarId = birthEvent.Event.EventRegistrar?.RegistrarInfo.Id
                            };
                            // Separate profile photos from supporting documents.
                            var (userPhotos,fingerprints, otherDocs) = _eventDocumentService.extractSupportingDocs(personIds, birthEvent.Event.EventSupportingDocuments);
                            // Save the profile photos.
                            _eventDocumentService.savePhotos(userPhotos);
                            // Save Other supporting documents and payment exemption documents.
                            _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)otherDocs, birthEvent.Event.PaymentExamption?.SupportingDocuments, "Birth");
                            _eventDocumentService.saveFingerPrints(fingerprints);
                            
                            // For non exempted documents 
                            if (!birthEvent.Event.IsExampted)
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
                                // }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            response.Success = false;
                            response.Status = 400;
                            throw;
                        }
                        response.Message = "Birth Event created Successfully";
                        response.Status = 200;
                        await transaction.CommitAsync();

                    }

                    return response;

                }
                catch (Exception)
                {
                    // Rollback the transaction on exception.
                    await transaction.RollbackAsync();
                    throw;
                }

            });
        }
    }
}
