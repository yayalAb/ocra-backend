using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Courts.Commmands.Create
{

    public record CreateCourtCommand(AddCourtRequest court) : IRequest<CreateCourtCommandResponse>
    {

    }
}

