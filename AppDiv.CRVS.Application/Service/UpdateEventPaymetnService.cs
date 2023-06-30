using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
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
        public UpdateEventPaymetnService(ICertificateRepository CertificateRepository,
                                         IAuthenticationRepository AuthenticationRequestRepostory,
                                         IEventRepository eventRepostory,
                                         ICorrectionRequestRepostory CorrectionRequestRepostory,
                                         IMediator mediator,
                                         IWorkflowService WorkflowService,
                                         IPaymentRequestRepository PaymentRequestRepository,
                                         IPaymentRepository paymentRepository,
                                         IContentValidator contentValidator)
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
                throw new Exception("Payment Request does't found");
            }
            try
            {
                requst.status = true;
                await _PaymentRequestRepository.UpdateAsync(requst, x => x.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "change" || requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "authentication")
            {
                if (requst.Request == null)
                {
                    throw new Exception("Request Does't Found");
                }

                var response = await _WorkflowService.ApproveService(requst.Request.Id, requst.Request.RequestType, true, "approved After payment", true, cancellationToken);
                if (response.Item1)
                {
                    if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "authentication")
                    {
                        var AuthRequ = _AuthenticationRequestRepostory.GetAll()
                        .Where(x => x.RequestId == requst.Request.Id).FirstOrDefault();
                        var certificate = _CertificateRepository.GetAll().Where(x => x.Id == AuthRequ.CertificateId).FirstOrDefault();
                        certificate.AuthenticationStatus = true;
                        _CertificateRepository.Update(certificate);
                    }
                    else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "change")
                    {
                        var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                                        .Include(x => x.Event)
                                        .Where(x => x.RequestId == requst.Request.Id)
                                        .Include(x => x.Request).FirstOrDefault();
                        var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                        _ = await _contentValidator.ValidateAsync(modifiedEvent.Event.EventType, CorrectionRequestResponse.Content);
                        // if (modifiedEvent.Event.EventType == "Adoption")
                        // {
                        //     UpdateAdoptionCommand AdoptionCommand = CorrectionRequestResponse.Content.ToObject<UpdateAdoptionCommand>();
                        //     AdoptionCommand.IsFromCommand = true;
                        //     var response1 = await _mediator.Send(AdoptionCommand);
                        // }
                        // else if (modifiedEvent.Event.EventType == "Birth")
                        // {
                        //     UpdateBirthEventCommand BirthCommand = CorrectionRequestResponse.Content.ToObject<UpdateBirthEventCommand>();
                        //     BirthCommand.IsFromCommand = true;
                        //     var response1 = await _mediator.Send(BirthCommand);
                        // }
                        // else if (modifiedEvent.Event.EventType == "Death")
                        // {
                        //     UpdateDeathEventCommand DeathCommand = CorrectionRequestResponse.Content.ToObject<UpdateDeathEventCommand>();
                        //     DeathCommand.IsFromCommand = true;
                        //     var response1 = await _mediator.Send(DeathCommand);
                        // }
                        // else if (modifiedEvent.Event.EventType == "Divorce")
                        // {
                        //     UpdateDivorceEventCommand DivorceCommand = CorrectionRequestResponse.Content.ToObject<UpdateDivorceEventCommand>();
                        //     DivorceCommand.IsFromCommand = true;
                        //     var response1 = await _mediator.Send(DivorceCommand);
                        // }
                        // else if (modifiedEvent.Event.EventType == "Marriage")
                        // {
                        //     UpdateMarriageEventCommand MarriageCommand = CorrectionRequestResponse.Content.ToObject<UpdateMarriageEventCommand>();
                        //     MarriageCommand.IsFromCommand = true;
                        //     var response1 = await _mediator.Send(MarriageCommand);
                        // }
                    }
                    else if (requst?.Request?.RequestType == "verfication")
                    {

                    }
                    // await _eventRepostory.SaveChangesAsync(cancellationToken);
                }
            }
            else if (requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "certificategeneration" ||
            requst?.PaymentRate?.PaymentTypeLookup?.Value?.Value<string>("en")?.ToLower() == "reprint")
            {

                await _paymentRepository.UpdateEventPaymentStatus(paymentRequestId);
            }
            else
            {
                throw new Exception("Unkown Payment Added");
            }
        }
    }
}