
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.Notifications.Commands.RemoveByRequestId

{
    public class RemoveNotificationByRequestIdCommand : IRequest<BaseResponse>
    {
        public Guid RequestId { get; set; }
    }

    public class RemoveNotificationByRequestIdCommandHandler : IRequestHandler<RemoveNotificationByRequestIdCommand, BaseResponse>
    {
        private readonly INotificationService _notificationService;

        public RemoveNotificationByRequestIdCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<BaseResponse> Handle(RemoveNotificationByRequestIdCommand request, CancellationToken cancellationToken)
        {
            //    await _notificationService.updateSeenStatus(request.Id);
            await _notificationService.RemoveNotificationByRequest(request.RequestId);
            return new BaseResponse { Message = "Notification removed successfully", Status = 200 };
        }
    }
}