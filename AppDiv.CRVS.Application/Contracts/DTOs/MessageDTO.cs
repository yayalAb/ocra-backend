using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class MessageDTO
    {
        public string Type { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string? TextMessage { get; set; }
        public Guid? ParentMessageId { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public DateTime CreatedAt {get; set; }
        public string CreatedAtEt {get;set; }
    }
}