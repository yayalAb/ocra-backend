
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Features.Messages.Query.GetMessagesByUserId
{
    // Customer GetMessagesByUserIdQuery with  response
    public class GetMessagesByUserIdQuery : IRequest<object>
    {
        public string UserId { get; set; }

    }

    public class GetMessagesByUserIdQueryHandler : IRequestHandler<GetMessagesByUserIdQuery, object>
    {

        private readonly IMessageRepository _MessageRepository;

        public GetMessagesByUserIdQueryHandler(IMessageRepository MessageQueryRepository)
        {
            _MessageRepository = MessageQueryRepository;
        }
        public async Task<object> Handle(GetMessagesByUserIdQuery request, CancellationToken cancellationToken)
        {

            return  _MessageRepository.GetAll()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Where(m => m.SenderId == request.UserId || m.ReceiverId == request.UserId)
            .OrderBy(m => m.CreatedAt)
            .GroupBy(m => new { senderId = m.SenderId, receiverId = m.ReceiverId })
            .Select(g => new ChatDTO
            {
                With = g.Key.senderId != request.UserId ? g.Key.senderId : g.Key.receiverId,
                Chats = g.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    Type = m.Type,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    TextMessage = m.TextMessage,
                    ParentMessageId = m.ParentMessageId,
                    SenderName = m.Sender.UserName,
                    ReceiverName = m.Receiver.UserName,
                    CreatedAt = m.CreatedAt,
                    CreatedAtEt = m.CreatedAt == null ? "" : new CustomDateConverter(m.CreatedAt).ethiopianDate
                })  .OrderBy(m => m.CreatedAt).ToList()
            }).ToList().GroupBy(c => c.With)
            .Select(g => new ChatDTO{
                With = g.Key,
                Chats = g.SelectMany(ch => ch.Chats).OrderBy(m => m.CreatedAt).ToList(),
            });
            // .ToListAsync();
        }
    }
}