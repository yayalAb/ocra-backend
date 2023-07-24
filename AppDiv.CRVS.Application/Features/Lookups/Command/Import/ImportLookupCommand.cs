using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Lookups.Command.Import
{
    // Customer Import command with CustomerResponse
    public record ImportLookupCommand(ICollection<AddLookupRequest> Lookups) : IRequest<ImportLookupCommadResponse>
    {

    }
}



