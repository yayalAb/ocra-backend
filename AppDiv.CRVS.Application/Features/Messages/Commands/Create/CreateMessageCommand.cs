using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.Messages.Command.Create
{
    // Customer create command with CustomerResponse
    public record CreateMessageCommand : IRequest<CreateMessageCommadResponse>
    {
        public string Type { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string? TextMessage { get; set; }
        public Guid? ParentMessageId { get; set; }

    }
}



