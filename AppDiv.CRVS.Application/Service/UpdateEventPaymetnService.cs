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
        private readonly IWorkflowService _WorkflowService;
        private readonly IEventRepository _eventRepostory;
        private readonly IMediator _mediator;
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        private readonly IAuthenticationRepository _AuthenticationRequestRepostory;
        private readonly ICertificateRepository _CertificateRepository;
        public UpdateEventPaymetnService(ICertificateRepository CertificateRepository, IAuthenticationRepository AuthenticationRequestRepostory, IEventRepository eventRepostory, ICorrectionRequestRepostory CorrectionRequestRepostory, IMediator mediator, IWorkflowService WorkflowService, IPaymentRequestRepository PaymentRequestRepository, IPaymentRepository paymentRepository)
        {
            _PaymentRequestRepository = PaymentRequestRepository;
            _paymentRepository = paymentRepository;
            _WorkflowService = WorkflowService;
            _mediator = mediator;
            _CorrectionRequestRepostory = CorrectionRequestRepostory;
            _eventRepostory = eventRepostory;
            _AuthenticationRequestRepostory = AuthenticationRequestRepostory;
            _CertificateRepository = CertificateRepository;

        }

        async void IUpdateEventPaymetnService.UpdatePaymetnStatus(Guid paymentRequestId, CancellationToken cancellationToken)
        {

            var requst = _PaymentRequestRepository
              .GetAll()
              .Include(x => x.Request)
              .Where(x => x.Id == paymentRequestId).FirstOrDefault();
            try
            {
                requst.status = true;
                await _PaymentRequestRepository.UpdateAsync(requst, x => x.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            if (requst?.Request?.RequestType == "change" || requst?.Request?.RequestType == "authentication")
            {
                var response = await _WorkflowService.ApproveService(requst.Request.Id, requst.Request.RequestType, true, "approved After payment", true, cancellationToken);
                if (response.Item1)
                {
                    Console.WriteLine("payment request Id  last step");

                    if (requst?.Request?.RequestType == "authentication")
                    {
                        Console.WriteLine("payment request Id  Authentntication");
                        var AuthRequ = _AuthenticationRequestRepostory.GetAll()
                        .Where(x => x.RequestId == requst.Request.Id).FirstOrDefault();
                        var certificate = _CertificateRepository.GetAll().Where(x => x.Id == AuthRequ.CertificateId).FirstOrDefault();
                        certificate.AuthenticationStatus = true;
                        _CertificateRepository.Update(certificate);
                    }
                    else if (requst?.Request?.RequestType == "change")
                    {
                        var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                                        .Include(x => x.Event)
                                        .Where(x => x.RequestId == requst.Request.Id)
                                        .Include(x => x.Request).FirstOrDefault();
                        var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                        if (modifiedEvent.Event.EventType == "Adoption")
                        {
                            UpdateAdoptionCommand AdoptionCommand = CorrectionRequestResponse.Content.ToObject<UpdateAdoptionCommand>();
                            AdoptionCommand.IsFromCommand = true;
                            var response1 = await _mediator.Send(AdoptionCommand);
                        }
                        else if (modifiedEvent.Event.EventType == "Birth")
                        {
                            UpdateBirthEventCommand BirthCommand = CorrectionRequestResponse.Content.ToObject<UpdateBirthEventCommand>();
                            BirthCommand.IsFromCommand = true;
                            var response1 = await _mediator.Send(BirthCommand);
                        }
                        else if (modifiedEvent.Event.EventType == "Death")
                        {
                            UpdateDeathEventCommand DeathCommand = CorrectionRequestResponse.Content.ToObject<UpdateDeathEventCommand>();
                            DeathCommand.IsFromCommand = true;
                            var response1 = await _mediator.Send(DeathCommand);
                        }
                        else if (modifiedEvent.Event.EventType == "Divorce")
                        {
                            UpdateDivorceEventCommand DivorceCommand = CorrectionRequestResponse.Content.ToObject<UpdateDivorceEventCommand>();
                            DivorceCommand.IsFromCommand = true;
                            var response1 = await _mediator.Send(DivorceCommand);
                        }
                        else if (modifiedEvent.Event.EventType == "Marriage")
                        {
                            UpdateMarriageEventCommand MarriageCommand = CorrectionRequestResponse.Content.ToObject<UpdateMarriageEventCommand>();
                            MarriageCommand.IsFromCommand = true;
                            var response1 = await _mediator.Send(MarriageCommand);
                        }
                    }
                    else if (requst?.Request?.RequestType == "verfication")
                    {

                    }
                    // await _eventRepostory.SaveChangesAsync(cancellationToken);
                }
            }
            else if (requst?.Request?.RequestType == "CertificateGeneration")
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