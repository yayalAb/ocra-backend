using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Utility.Contracts;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface INotificationService
    {
        
        public Task CreateNotification(Guid notificationObjId,  string type, string message, Guid groupId, Guid? requestId, string senderId,Guid? eventRegisteredAddressId);
        public Task updateSeenStatus(Guid notificationId);
        public Task<List<NotificationResponseDTO>> getNotification(List<Guid> groupIds);
        public Task updateSeenStatusByRequest(Guid requestId, Guid groupId, string type);
        public  Task RemoveNotification(Guid notificationId);


    }
}