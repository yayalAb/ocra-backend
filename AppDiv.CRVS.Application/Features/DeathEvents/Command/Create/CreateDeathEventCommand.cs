using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.DeathEvents.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateDeathEventCommand(AddDeathEventRequest DeathEvent) : IRequest<CreateDeathEventCommandResponse>
    {

    }
}