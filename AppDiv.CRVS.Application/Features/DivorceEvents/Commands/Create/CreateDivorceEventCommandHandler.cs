
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;

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
        private readonly ICourtRepository _courtRepository;

        public CreateDivorceEventCommandHandler(IDivorceEventRepository DivorceEventRepository,
                                                IPersonalInfoRepository personalInfoRepository,
                                                IEventDocumentService eventDocumentService,
                                                ILookupRepository lookupRepository,
                                                IAddressLookupRepository addressLookupRepository,
                                                IEventPaymentRequestService paymentRequestService,
                                                ISmsService smsService,
                                                ICourtRepository courtRepository)
        {
            _DivorceEventRepository = DivorceEventRepository;
            _personalInfoRepository = personalInfoRepository;
            _eventDocumentService = eventDocumentService;
            _lookupRepository = lookupRepository;
            _addressLookupRepository = addressLookupRepository;
            _paymentRequestService = paymentRequestService;
            _smsService = smsService;
            _courtRepository = courtRepository;
        }
        public async Task<CreateDivorceEventCommandResponse> Handle(CreateDivorceEventCommand request, CancellationToken cancellationToken)
        {
            var executionStrategy = _DivorceEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _DivorceEventRepository.Database.BeginTransaction())
                {
                    try
                    {
                        var createDivorceEventCommandResponse = new CreateDivorceEventCommandResponse();

                        var validator = new CreateDivorceEventCommandValidator(_personalInfoRepository, _lookupRepository, _addressLookupRepository, _courtRepository);
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


                            var divorceEvent = CustomMapper.Mapper.Map<DivorceEvent>(request);
                            divorceEvent.Event.EventType = "Divorce";
                            await _DivorceEventRepository.InsertOrUpdateAsync(divorceEvent, cancellationToken);
                            await _DivorceEventRepository.SaveChangesAsync(cancellationToken);
                            _eventDocumentService.saveSupportingDocuments(divorceEvent.Event.EventSupportingDocuments, divorceEvent.Event.PaymentExamption?.SupportingDocuments, "Divorce");
                            // create payment request for the event if it is not exempted
                            if (!divorceEvent.Event.IsExampted)
                            {
                                var amount = await _paymentRequestService.CreatePaymentRequest("Divorce", divorceEvent.Event, cancellationToken);
                                string message = $"Dear Customer,\nThis is to inform you that your request for Divorce certificate from OCRA is currently being processed. To proceed with the issuance, kindly make a payment of {amount} to finance office.\nThank you for choosing OCRA";
                                List<string> msgRecepients = new List<string>();
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
                            createDivorceEventCommandResponse.Message = "Divorce event created successfully";
                            await transaction.CommitAsync();
                        }
                        return createDivorceEventCommandResponse;
                    }
                    catch (System.Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            });


        }
    }
}
