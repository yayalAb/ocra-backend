
using System.Security.Cryptography;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly CRVSDbContext _context;

        public NotificationService(CRVSDbContext context)
        {
            _context = context;
        }
        public async Task CreateNotification(Guid? RequestId,Guid notificationObjId,  string type, string message, Guid groupId, Guid? requestId)
        {

            var notification = new Notification
            {
                Type = type,
                NotificationObjId = notificationObjId,
                MessageStr = message,
                RequestId = requestId,
                GroupId = groupId
            };
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            //TODO:send notification for users with the groupid

        }
        public async Task updateSeenStatus(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                throw new NotFoundException($"notification with id = {notificationId} is not found");
            }
            notification.Seen = true;
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync(); 
            

        }
        public async Task<List<NotificationResponseDTO>> getNotification(Guid groupId){
            return await _context.Notifications
                    .Where(n => n.GroupId == groupId && !n.Seen)
                    .Select(n =>new NotificationResponseDTO{
                        Type = n.Type,
                        MessageStr = n.MessageStr,
                        NotificationObjId = n.NotificationObjId,
                        RequestId = n.RequestId,
                        GroupId = n.GroupId

                    })
                    .ToListAsync();
        }

    }
}

