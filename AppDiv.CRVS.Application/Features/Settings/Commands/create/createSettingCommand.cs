using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Commands.create
{
    // Customer create command with CustomerResponse

    public record createSettingCommand(AddSettingRequest setting) : IRequest<createSettingCommandResponse>
    {

    }
}