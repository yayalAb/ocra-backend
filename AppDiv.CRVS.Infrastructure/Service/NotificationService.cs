
using System.Security.Cryptography;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
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
        public async Task CreateNotification(Guid notificationObjId,  string type, string message, Guid groupId, Guid? requestId, string senderId)
        {
   
            var notification = new Notification
            {
                Type = type,
                NotificationObjId = notificationObjId,
                MessageStr = message,
                RequestId = requestId,
                GroupId = groupId,
                SenderId = senderId
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
        public async Task<List<NotificationResponseDTO>> getNotification(List<Guid> groupIds){

            return await _context.Notifications
                    .Include(n => n.Sender)
                        .ThenInclude(s => s.PersonalInfo)
                    .Include(n => n.Request.CorrectionRequest)
                    .Include(n => n.Request.AuthenticationRequest)
                    .Where(n => groupIds.Contains(n.GroupId) && !n.Seen)
                    .Select(n =>new NotificationResponseDTO{
                        Type = n.Type,
                        EventId = n.Type == Enum.GetName<NotificationType>(NotificationType.correction)
                                            ? n.Request.CorrectionRequest.EventId
                                  :n.Type == Enum.GetName<NotificationType>(NotificationType.correction)
                                  ?n.Request.AuthenticationRequest.Certificate.EventId
                                  :null,
                        MessageStr = n.MessageStr,
                        NotificationObjId = n.NotificationObjId,
                        RequestId = n.RequestId,
                        GroupId = n.GroupId,
                        CreatedAt = n.CreatedAt,
                        SenderFullName = n.Sender.PersonalInfo.FirstNameLang+" "+
                                         n.Sender.PersonalInfo.MiddleNameLang+" "+
                                         n.Sender.PersonalInfo.LastNameLang,
                        SenderUserName = n.Sender.UserName


                    })
                    .ToListAsync();
        }

    }
}

