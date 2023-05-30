
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{

    public class CreateBirthEventCommandHandler : IRequestHandler<CreateBirthEventCommand, CreateBirthEventCommandResponse>
    {
        private readonly IBirthEventRepository _birthEventRepository;
        private readonly ILookupRepository _lookupRepository;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IAddressLookupRepository _addressRepository;
        private readonly IPersonalInfoRepository _person;
        private readonly IPaymentExamptionRequestRepository _paymentExamption;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly ISmsService _smsService;
        private readonly ILogger<CreateBirthEventCommandHandler> _logger;

        public CreateBirthEventCommandHandler(IBirthEventRepository birthEventRepository,
                                              IEventDocumentService eventDocumentService,
                                              ILookupRepository lookupRepository,
                                              IAddressLookupRepository addressRepository,
                                              IPersonalInfoRepository person,
                                              IPaymentExamptionRequestRepository paymentExamption,
                                              IEventPaymentRequestService paymentRequestService,
                                              ISmsService smsService,
                                              ILogger<CreateBirthEventCommandHandler> logger)
        {
            _eventDocumentService = eventDocumentService;
            _birthEventRepository = birthEventRepository;
            _addressRepository = addressRepository;
            _lookupRepository = lookupRepository;
            _person = person;
            _paymentExamption = paymentExamption;
            _paymentRequestService = paymentRequestService;
            _smsService = smsService;
            _logger = logger;
        }
        public async Task<CreateBirthEventCommandResponse> Handle(CreateBirthEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _birthEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _birthEventRepository.Database.BeginTransaction())
                {

                    try

                    {
                        var createBirthEventCommandResponse = new CreateBirthEventCommandResponse();

                        var validator = new CreateBirthEventCommandValidator((_lookupRepository, _addressRepository, _person, _paymentExamption), request);
                        var validationResult = await validator.ValidateAsync(request, cancellationToken);

                        //Check and log validation errors
                        if (validationResult.Errors.Count > 0)
                        {
                            createBirthEventCommandResponse.Success = false;
                            createBirthEventCommandResponse.Status = 400;
                            createBirthEventCommandResponse.ValidationErrors = new List<string>();
                            foreach (var error in validationResult.Errors)
                                createBirthEventCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                            createBirthEventCommandResponse.Message = createBirthEventCommandResponse.ValidationErrors[0];
                        }
                        if (createBirthEventCommandResponse.Success)
                        {
                            // var docs = await _groupRepository.GetMultipleUserGroups(request.UserGroups);

                            try
                            {
                                var birthEvent = CustomMapper.Mapper.Map<BirthEvent>(request.BirthEvent);

                                await _birthEventRepository.InsertOrUpdateAsync(birthEvent, cancellationToken);
                                var result = await _birthEventRepository.SaveChangesAsync(cancellationToken);

                                var supportingDocuments = birthEvent.Event.EventSupportingDocuments;
                                var examptionDocuments = birthEvent.Event.PaymentExamption?.SupportingDocuments;

                                _eventDocumentService.saveSupportingDocuments(supportingDocuments, examptionDocuments, "Birth");
                                if (!birthEvent.Event.IsExampted)
                                {
                                   var amount = await _paymentRequestService.CreatePaymentRequest("Birth", birthEvent.Event, cancellationToken);
                                    string message = $"Dear Customer,\nThis is to inform you that your request for Birth certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {amount} to finance office.\nThank you for choosing OCRA";
                                    List<string> msgRecepients = new List<string>();
                                    if (birthEvent.Mother.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(birthEvent.Mother.PhoneNumber);
                                    }
                                    if (birthEvent.Father?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(birthEvent.Father.PhoneNumber);
                                    }
                                    if (birthEvent.Event.EventRegistrar?.RegistrarInfo?.PhoneNumber != null)
                                    {
                                        msgRecepients.Add(birthEvent.Event.EventRegistrar.RegistrarInfo.PhoneNumber);
                                    }
                                    await _smsService.SendBulkSMS(msgRecepients, message);
                                    
                                }

                            }
                            catch (System.Exception ex)
                            {
                                createBirthEventCommandResponse.Success = false;
                                createBirthEventCommandResponse.Status = 400;
                                throw;
                            }
                            createBirthEventCommandResponse.Message = "Birth Event created Successfully";
                            createBirthEventCommandResponse.Status = 200;
                            await transaction.CommitAsync();
                        }
                        return createBirthEventCommandResponse;

                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }

            });
        }
    }
}
