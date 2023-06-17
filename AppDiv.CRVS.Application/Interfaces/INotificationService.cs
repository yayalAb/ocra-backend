using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Utility.Contracts;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface INotificationService
    {
        
        public Task CreateNotification(Guid notificationObjId,  string type, string message, Guid groupId, Guid? requestId, string senderId);
        public Task updateSeenStatus(Guid notificationId);
        public Task<List<NotificationResponseDTO>> getNotification(List<Guid> groupIds);

    }
}