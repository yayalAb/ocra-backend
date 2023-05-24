using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateBirthEventCommand(AddBirthEventRequest BirthEvent) : IRequest<CreateBirthEventCommandResponse>
    {

    }
}