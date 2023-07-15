
using System.Security.Cryptography;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using AppDiv.CRVS.Infrastructure.Hub;
using AppDiv.CRVS.Application.Contracts.DTOs;
// using AppDiv.CRVS.Utility.Hub;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly CRVSDbContext _context;
        private readonly IHubContext<MessageHub, IMessageHubClient> _messageHub;

        public NotificationService(CRVSDbContext context, IHubContext<MessageHub, IMessageHubClient> messageHub)
        {
            _context = context;
            _messageHub = messageHub;
        }
        public async Task CreateNotification(Guid notificationObjId, string type, string message, Guid groupId, Guid? requestId, string senderId)
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

            var resNotification = await _context.Notifications
                    .Include(n => n.Sender)
                        .ThenInclude(s => s.PersonalInfo)
                    .Include(n => n.Request.CorrectionRequest)
                    .Include(n => n.Request.AuthenticationRequest)
                    .Where(n => n.Id == notification.Id)
                    .Select(n => new NotificationResponseDTO
                    {
                        Id = n.Id,
                        Type = n.Type,
                        EventId = n.Type.ToLower() == Enum.GetName<NotificationType>(NotificationType.change)
                                            ? n.Request.CorrectionRequest.EventId
                                  : n.Type.ToLower() == Enum.GetName<NotificationType>(NotificationType.change)
                                  ? n.Request.AuthenticationRequest.Certificate.EventId
                                  : null,
                        MessageStr = n.MessageStr,
                        NotificationObjId = n.NotificationObjId,
                        RequestId = n.RequestId,
                        GroupId = n.GroupId,
                        CreatedAt = n.CreatedAt,
                        SenderFullName = n.Sender.PersonalInfo.FirstNameLang + " " +
                                         n.Sender.PersonalInfo.MiddleNameLang + " " +
                                         n.Sender.PersonalInfo.LastNameLang,
                        SenderUserName = n.Sender.UserName,
                        SenderId = n.SenderId


                    }).FirstOrDefaultAsync();
            if (resNotification == null)
            {
                throw new NotFoundException("notification could not be created");
            }
            // send notification to the group
            await _messageHub.Clients.Group(resNotification.GroupId.ToString()).NewNotification(resNotification);

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
        public async Task<List<NotificationResponseDTO>> getNotification(List<Guid> groupIds)
        {

            return await _context.Notifications
                    .Include(n => n.Sender)
                        .ThenInclude(s => s.PersonalInfo)
                    .Include(n => n.Request.CorrectionRequest)
                    .Include(n => n.Request.AuthenticationRequest)
                    .Where(n => groupIds.Contains(n.GroupId) && !n.Seen)
                    .Select(n => new NotificationResponseDTO
                    {
                        Id = n.Id,
                        Type = n.Type,
                        EventId = n.Type.ToLower() == Enum.GetName<NotificationType>(NotificationType.change)
                                            ? n.Request.CorrectionRequest.EventId
                                  : n.Type.ToLower() == Enum.GetName<NotificationType>(NotificationType.change)
                                  ? n.Request.AuthenticationRequest.Certificate.EventId
                                  : null,
                        MessageStr = n.MessageStr,
                        NotificationObjId = n.NotificationObjId,
                        RequestId = n.RequestId,
                        GroupId = n.GroupId,
                        CreatedAt = n.CreatedAt,
                        SenderFullName = n.Sender.PersonalInfo.FirstNameLang + " " +
                                         n.Sender.PersonalInfo.MiddleNameLang + " " +
                                         n.Sender.PersonalInfo.LastNameLang,
                        SenderUserName = n.Sender.UserName,
                        SenderId = n.SenderId


                    })
                    .ToListAsync();
        }


    }
}

