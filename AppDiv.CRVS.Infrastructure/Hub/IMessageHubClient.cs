using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Utility.Contracts;

namespace AppDiv.CRVS.Infrastructure.Hub;
public interface IMessageHubClient
{
    Task SendNotification(List<NotificationResponseDTO> notifications);
    Task SendSingleNotification(NotificationResponseDTO notification);

}