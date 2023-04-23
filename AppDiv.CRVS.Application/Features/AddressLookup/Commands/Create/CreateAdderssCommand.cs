using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public record CreateAdderssCommand(AddAddressRequest Address) : IRequest<CreateAdderssCommandResponse>
    {

    }
}