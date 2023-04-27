
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.WorkFlows.Query.GetAllWorkFlow;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Query.GetWorkFlowById
{
    // Customer GetWorkFlowByIdQuery with  response
    public class GetWorkFlowByIdQuery : IRequest<WorkflowDTO>
    {
        public Guid Id { get; private set; }

        public GetWorkFlowByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetWorkFlowByIdQueryHandler : IRequestHandler<GetWorkFlowByIdQuery, WorkflowDTO>
    {
        private readonly IWorkflowRepository _workflowRepository;

        public GetWorkFlowByIdQueryHandler(IWorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }
        public async Task<WorkflowDTO> Handle(GetWorkFlowByIdQuery request, CancellationToken cancellationToken)
        {
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "Steps", NavigationPropertyType.COLLECTION }

                                                };
            var workflows = await _workflowRepository.GetWithAsync(request.Id, explicitLoadedProperties);
            // var selectedworkflow = workflows.FirstOrDefault(x => x.Id == request.Id);
            return CustomMapper.Mapper.Map<WorkflowDTO>(workflows);
            // return selectedCustomer;
        }
    }
}