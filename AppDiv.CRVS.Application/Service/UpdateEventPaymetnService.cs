using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
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
        private readonly IMediator _mediator;
        private readonly ICorrectionRequestRepostory _CorrectionRequestRepostory;
        public UpdateEventPaymetnService(ICorrectionRequestRepostory CorrectionRequestRepostory, IMediator mediator, IWorkflowService WorkflowService, IPaymentRequestRepository PaymentRequestRepository, IPaymentRepository paymentRepository)
        {
            _PaymentRequestRepository = PaymentRequestRepository;
            _paymentRepository = paymentRepository;
            _WorkflowService = WorkflowService;
            _mediator = mediator;
            _CorrectionRequestRepostory = CorrectionRequestRepostory;

        }

        async void IUpdateEventPaymetnService.UpdatePaymetnStatus(Guid paymentRequestId, CancellationToken cancellationToken)
        {
            var requst = _PaymentRequestRepository
              .GetAll()
              .Include(x => x.Request)
              .Where(x => x.Id == paymentRequestId).FirstOrDefault();
            Console.WriteLine("Payment Type selected {0} ", requst?.Request?.RequestType);
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
                Console.WriteLine("Payment Type selected aaa");

                if (response.Item1)
                {
                    if (requst?.Request?.RequestType == "authentication")
                    {

                    }
                    else if (requst?.Request?.RequestType == "change")
                    {
                        Console.WriteLine("paument Adding");
                        var modifiedEvent = _CorrectionRequestRepostory.GetAll()
                            .Where(x => x.RequestId == requst.Request.Id)
                            .Include(x => x.Request).FirstOrDefault();
                        var CorrectionRequestResponse = CustomMapper.Mapper.Map<AddCorrectionRequest>(modifiedEvent);
                        UpdateAdoptionCommand AdoptionCommand = CorrectionRequestResponse.Content.ToObject<UpdateAdoptionCommand>();
                        var adoption = await _mediator.Send(AdoptionCommand);
                    }
                    else if (requst?.Request?.RequestType == "verfication")
                    {

                    }
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