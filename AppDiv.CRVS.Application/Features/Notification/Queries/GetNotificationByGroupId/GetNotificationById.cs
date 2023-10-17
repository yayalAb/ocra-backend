
using System;
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Notifications.Queries.GetNotificationByTransactionId

{
    public class GetNotificationByIdQuery : IRequest<NotificationData>
    {
        public Guid Id { get; set; }
    }

    public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationData>
    {
        private readonly INotificationService _notificationService;

        public GetNotificationByIdQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<NotificationData> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
        {
           return await _notificationService.GetNotification(request.Id);
        }
    }
}