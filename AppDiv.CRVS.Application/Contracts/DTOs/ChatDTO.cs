using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ChatDTO
    {
        public string With {get; set;}
        public List<MessageDTO> Chats {get; set;}
    }
}