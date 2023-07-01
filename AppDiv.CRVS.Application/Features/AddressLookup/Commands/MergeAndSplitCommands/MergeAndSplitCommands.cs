
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.MergeAndSplitCommands
{
    public record MergeAndSplitCommand(List<AddAddressRequest> Address) : IRequest<MergeAndSplitCommandResponse>
    {

    }
}