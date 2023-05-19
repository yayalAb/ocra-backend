using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create
{
    public record CreateAdoptionCommand(AddAdoptionRequest Adoption) : IRequest<CreateAdoptionCommandResponse>
    {

    }
}

