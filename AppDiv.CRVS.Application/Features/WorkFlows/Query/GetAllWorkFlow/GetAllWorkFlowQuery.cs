
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
    public record GetAllWorkFlowQuery : IRequest<List<GetAllWorkFlowDTO>>
    {

    }

    public class GetAllWorkFlowQueryHandler : IRequestHandler<GetAllWorkFlowQuery, List<GetAllWorkFlowDTO>>
    {
        private readonly IStepRepository _workflowRepository;

        public GetAllWorkFlowQueryHandler(IStepRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<List<GetAllWorkFlowDTO>> Handle(GetAllWorkFlowQuery request, CancellationToken cancellationToken)
        {
            var LookupList = await _workflowRepository.GetAllWithAsync("workflow");
            var workflow = LookupList.Select(wf => new GetAllWorkFlowDTO
            {
                id = wf.Id,
                workflowName = wf.workflow.workflowName,
                step = wf.step,
                payment = wf.Payment,
                responsibleGroup = wf.ResponsibleGroup,
                status = wf.Status
            });
            return workflow.ToList();
        }
    }
}

