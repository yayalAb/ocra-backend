using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateLookupCommand(AddLookupRequest lookup) : IRequest<CreateLookupCommadResponse>
    {

    }
}



