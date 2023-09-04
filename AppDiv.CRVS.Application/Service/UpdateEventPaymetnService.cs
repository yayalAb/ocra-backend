using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
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

            if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "change"
             || requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "authentication"
             || requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "verfication"
             || requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "reprint"
             )
            {
                if (requst.Request == null)
                {
                    throw new NotFoundException("Request Does't Found");
                }
                var workflow = _WorkflowRepo.GetAll()
                .Include(x => x.Steps)
                .Where(x => x.workflowName == requst.Request.RequestType).FirstOrDefault();

                var response = await _WorkflowService.ApproveService(requst.Request.Id, requst.Request.RequestType, true, "approved After payment", null, true, cancellationToken);
                
                if (response.Item1 || workflow?.Steps?.FirstOrDefault() == null)
                {
                    if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "authentication")
                    {
                        var AuthRequ = _AuthenticationRequestRepostory.GetAll()
                        .Where(x => x.RequestId == requst.Request.Id).FirstOrDefault();
                        var certificate = _CertificateRepository.GetAll().Where(x => x.Id == AuthRequ.CertificateId).FirstOrDefault();
                        certificate.AuthenticationStatus = true;
                        certificate.AuthenticationAt=DateTime.Now;
                        _CertificateRepository.Update(certificate);
                    }
                    else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "change")
                    {
                        var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                                        .Include(x => x.Event)
                                        .Where(x => x.RequestId == requst.Request.Id)
                                        .Include(x => x.Request).FirstOrDefault();
                        var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                        await _contentValidator.ValidateAsync(modifiedEvent.Event.EventType, CorrectionRequestResponse.Content, false);
                    }
                    else if (requst?.Request?.RequestType == "verfication")
                    {
                        var selectedEvent = await _eventRepostory.GetAsync(requst.EventId);
                        selectedEvent.IsVerified = true;
                        await _eventRepostory.UpdateAsync(selectedEvent, x => x.Id);
                    }
                     else if (requst?.Request?.RequestType == "reprint")
                    {
                         await _paymentRepository.UpdateEventPaymentStatus(paymentRequestId);
                    }
                    // await _eventRepostory.SaveChangesAsync(cancellationToken);
                }
            }
            else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "certificategeneration")
            {

                await _paymentRepository.UpdateEventPaymentStatus(paymentRequestId);
            }
        }
    }
}