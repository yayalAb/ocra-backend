
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
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Utility.Services;
// using AppDiv.CRVS.Utility.Hub;

namespace AppDiv.CRVS.Infrastructure.Service
{
    public class NotificationService : INotificationService
    {
        private readonly CRVSDbContext _context;
        private readonly IHubContext<MessageHub, IMessageHubClient> _messageHub;
        private readonly IDateAndAddressService _addressService;
        private readonly IUserResolverService _userResolverService;

        public NotificationService(CRVSDbContext context, IHubContext<MessageHub, IMessageHubClient> messageHub, IDateAndAddressService addressService, IUserResolverService userResolverService)
        {
            _context = context;
            _messageHub = messageHub;
            _addressService = addressService;
            _userResolverService = userResolverService;
        }
        public async Task CreateNotification(Guid notificationObjId, string type, string message, Guid? groupId, Guid? requestId, string senderId, Guid? eventRegisteredAddressId, string approvalType, string? receiverId = null)
        {

            var notification = new Notification
            {
                Type = type,
                NotificationObjId = notificationObjId,
                MessageStr = message,
                RequestId = requestId,
                GroupId = groupId,
                SenderId = senderId,
                ReceiverId = receiverId,
                ApprovalType = approvalType,
                EventRegisteredAddressId = eventRegisteredAddressId
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
                                  : n.Type.ToLower() == Enum.GetName<NotificationType>(NotificationType.authentication)
                                  ? n.Request.AuthenticationRequest.Certificate.EventId
                                  : n.Type.ToLower() == Enum.GetName<NotificationType>(NotificationType.verification)
                                  ? n.Request.VerficationRequest.EventId
                                  : null,
                        MessageStr = n.MessageStr,
                        NotificationObjId = n.NotificationObjId,
                        RequestId = n.RequestId,
                        GroupId = n.GroupId,
                        ReceiverId = n.ReceiverId,
                        CreatedAt = new CustomDateConverter( n.CreatedAt).ethiopianDate,
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
            if (!string.IsNullOrEmpty(receiverId))// send notification to single user
            {
                await _messageHub.Clients.User(receiverId).NewNotification(resNotification);
            }
            else if (eventRegisteredAddressId != null)
            {

                AddressResponseDTOE addressResponse = await _addressService.FormatedAddressLoop(eventRegisteredAddressId);
                // send notification to the group 
                if (addressResponse?.Country != null)
                    await _messageHub.Clients.Group(resNotification.GroupId.ToString() + "_" + addressResponse?.Country).NewNotification(resNotification);
                if (addressResponse?.Region != null)
                    await _messageHub.Clients.Group(resNotification.GroupId.ToString() + "_" + addressResponse?.Region).NewNotification(resNotification);
                if (addressResponse?.Zone != null)
                    await _messageHub.Clients.Group(resNotification.GroupId.ToString() + "_" + addressResponse?.Zone).NewNotification(resNotification);
                if (addressResponse?.Woreda != null)
                    await _messageHub.Clients.Group(resNotification.GroupId.ToString() + "_" + addressResponse?.Woreda).NewNotification(resNotification);
                if (addressResponse?.Kebele != null)
                    await _messageHub.Clients.Group(resNotification.GroupId.ToString() + "_" + addressResponse?.Kebele).NewNotification(resNotification);
            }


        }

        public async Task<NotificationData?> GetNotification(Guid requestId)
        {
            var notification =  await _context.Requests
                                    .Include(r => r.Notification)
                                    .ThenInclude(n => n.Sender)
                                    .ThenInclude(s => s.PersonalInfo)
                                    .Select(r => r.Notification)
                                    .FirstOrDefaultAsync();
            
            return notification == null ? null :
             new NotificationData
                {
                    Message = notification.MessageStr,
                    ApprovalType = notification.ApprovalType,
                    SenderId = notification.SenderId,
                    SenderUserName = notification.Sender.UserName,
                    SenderFullName = notification.Sender.PersonalInfo.FullNameLang,
                    Date = (new CustomDateConverter(notification.CreatedAt)).ethiopianDate
                };
            
        }
        public async Task RemoveNotification(Guid notificationId)
        {
            var notification = await _context.Notifications.Where(n => n.Id == notificationId).FirstOrDefaultAsync();
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                
                if (notification.EventRegisteredAddressId != null)
                {

                    var addressResponse = await _addressService.FormatedAddressLoop(notification.EventRegisteredAddressId);



                    // send remove notification  to the group 
                    if (addressResponse?.Country != null)
                        await _messageHub.Clients.Group(notification.GroupId.ToString() + "_" + addressResponse.Country).RemoveNotification(notificationId);
                    if (addressResponse?.Region != null)
                        await _messageHub.Clients.Group(notification.GroupId.ToString() + "_" + addressResponse.Region).RemoveNotification(notificationId);
                    if (addressResponse?.Zone != null)
                        await _messageHub.Clients.Group(notification.GroupId.ToString() + "_" + addressResponse.Zone).RemoveNotification(notificationId);
                    if (addressResponse?.Woreda != null)
                        await _messageHub.Clients.Group(notification.GroupId.ToString() + "_" + addressResponse.Woreda).RemoveNotification(notificationId);
                    if (addressResponse?.Kebele != null)
                        await _messageHub.Clients.Group(notification.GroupId.ToString() + "_" + addressResponse.Kebele).RemoveNotification(notificationId);
                }
            }
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
        public async Task updateSeenStatusByRequest(Guid requestId, Guid groupId, string type)
        {
            var notifications = await _context.Notifications
                                    .Where(n => n.RequestId == requestId && n.GroupId == groupId && n.Type == type)
                                    .ToListAsync();
            notifications.ForEach(n =>
            {
                n.Seen = true;
            });
            _context.Notifications.UpdateRange(notifications);
            await _context.SaveChangesAsync();


        }

        public async Task<List<NotificationResponseDTO>> getNotification(List<Guid> groupIds)
        {
            var workingAddressId = _userResolverService.GetWorkingAddressId();
            var userId = _userResolverService.GetUserId();
            if (workingAddressId == Guid.Empty) throw new NotFoundException("Invalid working address please login first");
            var query1 = _context.Notifications
                    .Include(n => n.Sender)
                        .ThenInclude(s => s.PersonalInfo)
                    .Include(n => n.Request.CorrectionRequest)
                    .Include(n => n.Request.AuthenticationRequest)
                    .Include(n => n.EventRegisteredAddress)
                    .Where(n =>(n.GroupId != null? groupIds.Contains((Guid)n.GroupId):n.ReceiverId == userId)  && !n.Seen)
                    .Where(n =>
                     (n.EventRegisteredAddress.AdminLevel >= 4 && n.EventRegisteredAddress.ParentAddress.ParentAddress.ParentAddress.Id == workingAddressId)
                     || (n.EventRegisteredAddress.AdminLevel >= 3 && n.EventRegisteredAddress.ParentAddress.ParentAddress.Id == workingAddressId)
                     || (n.EventRegisteredAddress.AdminLevel >= 2 && n.EventRegisteredAddress.ParentAddress.Id == workingAddressId)
                     || (n.EventRegisteredAddress.Id == workingAddressId));
            return await query1
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
                        CreatedAt =  new CustomDateConverter( n.CreatedAt).ethiopianDate,
                        SenderFullName = n.Sender.PersonalInfo.FirstNameLang + " " +
                                         n.Sender.PersonalInfo.MiddleNameLang + " " +
                                         n.Sender.PersonalInfo.LastNameLang,
                        SenderUserName = n.Sender.UserName,
                        SenderId = n.SenderId,
                        ApprovalType = n.ApprovalType,
                        hasApproval = n.GroupId != null && n.ReceiverId == null


                    })
                    .ToListAsync();
        }



    }
}

