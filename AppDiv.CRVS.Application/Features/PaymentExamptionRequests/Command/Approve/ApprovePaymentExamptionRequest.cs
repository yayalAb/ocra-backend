using System;
using System.Collections.Generic;
using System.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Approve
{
    // Customer create command with CustomerResponse
    public class PaymentExamptionApproval : IRequest<BaseResponse>
    {
        public Guid RequestId { get; set; }
        public string? Remark { get; set; }
        public bool IsApprove { get; set; }
        public Guid? ReasonLookupId { get; set; }
    }
    public class PaymentExamptionApprovalHandler : IRequestHandler<PaymentExamptionApproval, BaseResponse>
    {
        private readonly IPaymentExamptionRequestRepository _PaymentExamptionRepository;
        private readonly IWorkflowService _WorkflowService;
        public PaymentExamptionApprovalHandler(IWorkflowService WorkflowService, IPaymentExamptionRequestRepository PaymentExamptionRepository)
        {
            _PaymentExamptionRepository = PaymentExamptionRepository;
            _WorkflowService = WorkflowService;
        }
        public async Task<BaseResponse> Handle(PaymentExamptionApproval request, CancellationToken cancellationToken)
        {
            var response = await _WorkflowService.ApproveService(request.RequestId, "payment exemption", request.IsApprove, request.Remark, request.ReasonLookupId, false, cancellationToken);
            if (response.Item1)
            {
                try
                {
                    var PaymentExamptionRequest = await _PaymentExamptionRepository.GetAsync(response.Item2);
                    PaymentExamptionRequest.status = true;
                    await _PaymentExamptionRepository.UpdateAsync(PaymentExamptionRequest, x => x.Id);
                    await _PaymentExamptionRepository.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exp)
                {
                    throw new ApplicationException(exp.Message);
                }
            }
            return new BaseResponse
            {
                Message = "Approved Successfully"
            };
        }
    }
}