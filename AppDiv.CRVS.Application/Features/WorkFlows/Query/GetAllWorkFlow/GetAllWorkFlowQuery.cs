
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Query.GetAllWorkFlow

{
    // Customer query with List<Customer> response
    public record GetAllWorkFlowQuery : IRequest<PaginatedList<GetAllWorkFlowDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllWorkFlowQueryHandler : IRequestHandler<GetAllWorkFlowQuery, PaginatedList<GetAllWorkFlowDTO>>
    {
        private readonly IStepRepository _workflowRepository;

        public GetAllWorkFlowQueryHandler(IStepRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<PaginatedList<GetAllWorkFlowDTO>> Handle(GetAllWorkFlowQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<GetAllWorkFlowDTO>
                            .CreateAsync(
                                 _workflowRepository.GetAll()
                                .Select(wf => new GetAllWorkFlowDTO
                                {
                                    id = wf.workflow.Id,
                                    workflowName = wf.workflow.workflowName,
                                    step = wf.step,
                                    ResiponsbleGroup = wf.UserGroup.GroupName != null ? wf.UserGroup.GroupName : "",
                                    HasPayment = wf.workflow.HasPayment,
                                    status = wf.Status,
                                    PaymentStep = wf.workflow.PaymentStep
                                }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}

