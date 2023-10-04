using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Service
{
    public class UpdateEventPaymetnService : IUpdateEventPaymetnService
    {
        private readonly IPaymentRequestRepository _PaymentRequestRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IContentValidator _contentValidator;
        private readonly IWorkflowService _WorkflowService;
        private readonly IEventRepository _eventRepostory;
        private readonly IMediator _mediator;
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        private readonly IAuthenticationRepository _AuthenticationRequestRepostory;
        private readonly ICertificateRepository _CertificateRepository;
        private readonly IWorkflowRepository _WorkflowRepo;
        public UpdateEventPaymetnService(ICertificateRepository CertificateRepository,
                                         IAuthenticationRepository AuthenticationRequestRepostory,
                                         IEventRepository eventRepostory,
                                         ICorrectionRequestRepostory CorrectionRequestRepostory,
                                         IMediator mediator,
                                         IWorkflowService WorkflowService,
                                         IPaymentRequestRepository PaymentRequestRepository,
                                         IPaymentRepository paymentRepository,
                                         IContentValidator contentValidator,
                                         IWorkflowRepository WorkflowRepo
                                         )
        {
            _PaymentRequestRepository = PaymentRequestRepository;
            _paymentRepository = paymentRepository;
            this._contentValidator = contentValidator;
            _WorkflowService = WorkflowService;
            _mediator = mediator;
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _eventRepostory = eventRepostory;
            _AuthenticationRequestRepostory = AuthenticationRequestRepostory;
            _CertificateRepository = CertificateRepository;
            _WorkflowRepo = WorkflowRepo;

        }

        async Task IUpdateEventPaymetnService.UpdatePaymetnStatus(Guid paymentRequestId, CancellationToken cancellationToken)
        {

            var requst = _PaymentRequestRepository
              .GetAll()
              .Include(x => x.Request)
              .Include(x => x.PaymentRate)
              .ThenInclude(x => x.PaymentTypeLookup)
              .Include(x => x.PaymentRate.EventLookup)
              .Where(x => x.Id == paymentRequestId).FirstOrDefault();
            if (requst == null)
            {
                throw new NotFoundException("Payment Request does't found");
            }
            try
            {
                requst.status = true;
                await _PaymentRequestRepository.UpdateAsync(requst, x => x.Id);
            }
            catch (NotFoundException ex)
            {
                throw new NotFoundException(ex.Message);
            }

            string? paymentType = requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower();
            string [] validPaymetn = {"change","authentication","verfication","reprint"};

            if (validPaymetn.Contains(paymentType))
            {
                var workflow=new Workflow();
                (bool, Guid) response=(false,Guid.Empty);
                if (requst.Request != null)
                {
                workflow = _WorkflowRepo.GetAll()
                .Include(x => x.Steps)
                .Where(x => x.workflowName == requst.Request.RequestType).FirstOrDefault();
                 response = await _WorkflowService.ApproveService(requst.Request.Id, requst.Request.RequestType, true, "approved After payment", null, null, true, cancellationToken);
                }
                if (response.Item1 || workflow?.Steps?.FirstOrDefault() == null||requst.Request==null)
                {
                    if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "authentication" &&requst.Request != null)
                    {
                        var AuthRequ = _AuthenticationRequestRepostory.GetAll()
                        .Where(x => x.RequestId == requst.Request.Id).FirstOrDefault();
                        var certificate = _CertificateRepository.GetAll().Where(x => x.Id == AuthRequ.CertificateId).FirstOrDefault();
                        certificate.AuthenticationStatus = true;
                        certificate.AuthenticationAt=DateTime.Now;
                        _CertificateRepository.Update(certificate);
                    }
                    else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "change" && requst.Request != null)
                    {
                        var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                                        .Include(x => x.Event)
                                        .Where(x => x.RequestId == requst.Request.Id)
                                        .Include(x => x.Request).FirstOrDefault();
                        var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                        await _contentValidator.ValidateAsync(modifiedEvent.Event.EventType, CorrectionRequestResponse.Content, false);
                    }
                    else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "verfication" &&requst.Request != null)
                    {
                        var selectedEvent = await _eventRepostory.GetAsync(requst.EventId);
                        selectedEvent.IsVerified = true;
                        await _eventRepostory.UpdateAsync(selectedEvent, x => x.Id);
                    }
                     else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "reprint")
                    {
                         await _paymentRepository.UpdateEventPaymentStatus(paymentRequestId);
                    }
                }
            }
            else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "certificategeneration")
            {

                await _paymentRepository.UpdateEventPaymentStatus(paymentRequestId);
            }
        }
    }
}