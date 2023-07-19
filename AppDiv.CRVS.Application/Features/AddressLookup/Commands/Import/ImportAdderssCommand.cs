using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Import
{
    public record ImportAdderssCommand(ICollection<AddAddressRequest> Addresses) : IRequest<ImportAdderssCommandResponse>
    {

    }
}