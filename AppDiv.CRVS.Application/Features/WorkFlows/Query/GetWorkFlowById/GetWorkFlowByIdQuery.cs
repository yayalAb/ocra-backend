
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.WorkFlows.Query.GetAllWorkFlow;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
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
        private readonly IMediator _mediator;

        public GetWorkFlowByIdQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<WorkflowDTO> Handle(GetWorkFlowByIdQuery request, CancellationToken cancellationToken)
        {
            var workflows = await _mediator.Send(new GetAllWorkFlowQuery());
            var selectedworkflow = workflows.FirstOrDefault(x => x.id == request.Id);
            return CustomMapper.Mapper.Map<WorkflowDTO>(selectedworkflow);
            // return selectedCustomer;
        }
    }
}