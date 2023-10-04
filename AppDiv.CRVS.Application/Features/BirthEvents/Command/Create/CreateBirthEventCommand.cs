using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.BirthEvents.Command.Create
{
    // Birth Event create command
    public record CreateBirthEventCommand(AddBirthEventRequest BirthEvent) : IRequest<CreateBirthEventCommandResponse>
    {
    }
}