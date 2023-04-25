
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
    public record GetAllWorkFlowQuery : IRequest<List<WorkflowDTO>>
    {

    }

    public class GetAllWorkFlowQueryHandler : IRequestHandler<GetAllWorkFlowQuery, List<WorkflowDTO>>
    {
        private readonly IWorkflowRepository _workflowRepository;

        public GetAllWorkFlowQueryHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<List<WorkflowDTO>> Handle(GetAllWorkFlowQuery request, CancellationToken cancellationToken)
        {

            var LookupList = await _workflowRepository.GetAllWithAsync("Steps");
            // var LookupList = await _workflowRepository.GetAllAsync();
            var lookups = CustomMapper.Mapper.Map<List<WorkflowDTO>>(LookupList);
            return lookups;

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}