using AppDiv.CRVS.Utility.Contracts;

namespace AppDiv.CRVS.Utility.Hub;
public interface IMessageHubClient
{
    Task SendNotification(List<NotificationResponseDTO> notifications);
}