
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;
using AppDiv.CRVS.Application.Contracts.DTOs;

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

        public CreateDivorceEventCommandHandler(IDivorceEventRepository DivorceEventRepository,
                                                IPersonalInfoRepository personalInfoRepository,
                                                IEventDocumentService eventDocumentService,
                                                ILookupRepository lookupRepository,
                                                IAddressLookupRepository addressLookupRepository,
                                                IEventPaymentRequestService paymentRequestService,
                                                ISmsService smsService,
                                                IEventRepository eventRepository,
                                                ICourtRepository courtRepository)
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
        }
        public async Task<CreateDivorceEventCommandResponse> Handle(CreateDivorceEventCommand request, CancellationToken cancellationToken)
        {
            float amount = 0;
            var executionStrategy = _DivorceEventRepository.Database.CreateExecutionStrategy();
            return await executionStrategy.ExecuteAsync(async () =>
            {

                using (var transaction = _DivorceEventRepository.Database.BeginTransaction())
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
                            request.Event.EventDateEt = request?.CourtCase?.ConfirmedDateEt!;
                            var divorceEvent = CustomMapper.Mapper.Map<DivorceEvent>(request);
                            divorceEvent.Event.EventType = "Divorce";

                            var personIds = new PersonIdObj
                            {
                                WifeId = divorceEvent.DivorcedWife.Id,
                                HusbandId = divorceEvent.Event.EventOwener.Id,
                            };
                            await _DivorceEventRepository.InsertOrUpdateAsync(divorceEvent, cancellationToken);
                            var separatedDocs = _eventDocumentService.extractSupportingDocs(personIds, divorceEvent.Event.EventSupportingDocuments);
                            _eventDocumentService.savePhotos(separatedDocs.userPhotos);
                            _eventDocumentService.saveSupportingDocuments((ICollection<SupportingDocument>)separatedDocs.otherDocs, divorceEvent.Event.PaymentExamption?.SupportingDocuments, "Divorce");
                            _eventDocumentService.saveFingerPrints(separatedDocs.fingerPrint);
                        
                            // create payment request for the event if it is not exempted
                            if (!divorceEvent.Event.IsExampted)
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
                            await transaction.CommitAsync();
                            // }
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
