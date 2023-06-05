
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Notification.Queries.GetNotificationByGroupId

{
    public class GetNotificationByGroupIdQuery : IRequest<List<NotificationResponseDTO>>
    {
        public Guid GroupId { get; set; }
    }

    public class GetNotificationByGroupIdQueryHandler : IRequestHandler<GetNotificationByGroupIdQuery, List<NotificationResponseDTO>>
    {
        private readonly INotificationService _notificationService;

        public GetNotificationByGroupIdQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<List<NotificationResponseDTO>> Handle(GetNotificationByGroupIdQuery request, CancellationToken cancellationToken)
        {
           
           return await _notificationService.getNotification(request.GroupId);
        }
    }
}