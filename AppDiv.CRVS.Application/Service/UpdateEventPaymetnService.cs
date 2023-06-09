using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Service
{
    public class UpdateEventPaymetnService : IUpdateEventPaymetnService
    {
        private readonly IPaymentRequestRepository _PaymentRequestRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWorkflowService _WorkflowService;
        public UpdateEventPaymetnService(IWorkflowService WorkflowService, IPaymentRequestRepository PaymentRequestRepository, IPaymentRepository paymentRepository)
        {
            _PaymentRequestRepository = PaymentRequestRepository;
            _paymentRepository = paymentRepository;
            _WorkflowService = WorkflowService; ;
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
                await _WorkflowService.ApproveService(requst.Request.Id, requst.Request.RequestType, true, "approved After payment", true, cancellationToken);

            }
            else if (requst?.Request?.RequestType == "CertificateGeneration")
            {
                await _paymentRepository.UpdateEventPaymentStatus(paymentRequestId);

            }
        }
    }
}