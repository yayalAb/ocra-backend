using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Infrastructure.Hub.ChatHub
{
    public interface IChatHubClient
    {
        public Task SendChatAsync(MessageDTO message);


    }
}