
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.MarigeAndSplitCommands
{
    public record MerigeAndSplitCommand(AddAddressRequest Address) : IRequest<MerigeAndSplitCommandResponse>
    {

    }
}