
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

namespace AppDiv.CRVS.Application.Features.Messages.Query.GetMessagesByUserId
{
    // Customer GetMessagesByUserIdQuery with  response
    public class GetMessagesByUserIdQuery : IRequest<List<MessageDTO>>
    {
        public string UserId { get;  set; }

    }

    public class GetMessagesByUserIdQueryHandler : IRequestHandler<GetMessagesByUserIdQuery, List<MessageDTO>>
    {

        private readonly IMessageRepository _MessageRepository;

        public GetMessagesByUserIdQueryHandler(IMessageRepository MessageQueryRepository)
        {
            _MessageRepository = MessageQueryRepository;
        }
        public async Task<List<MessageDTO>> Handle(GetMessagesByUserIdQuery request, CancellationToken cancellationToken)
        {
        
           return await _MessageRepository.GetAll()
           .Include(m => m.Sender)
           .Include(m => m.Receiver)
           .Where(m => m.SenderId == request.UserId || m.ReceiverId == request.UserId)
           .Select(m => new MessageDTO{

           })
           .ToListAsync();
        }
    }
}