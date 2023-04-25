using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;

namespace AppDiv.CRVS.Application.Features.WorkFlows.Commands.Create
{
    public record CreateWorkFlowCommand(WorkflowAddRequest workflow) : IRequest<CreateWorkFlowCommandResponse>
    {

    }
}