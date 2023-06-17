using AppDiv.CRVS.Utility.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace AppDiv.CRVS.Utility.Hub;
public class MessageHub : Hub<IMessageHubClient>
{
    public async Task SendNotification(List<NotificationResponseDTO> notifications)
    {
        await  Clients.All.SendNotification(notifications);
    }
}