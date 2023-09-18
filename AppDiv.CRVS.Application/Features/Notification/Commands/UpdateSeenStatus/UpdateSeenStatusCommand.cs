
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.Application.Features.Notification.Commands.UpdateSeenStatus

{
    public class UpdateSeenStatusCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
    }

    public class UpdateSeenStatusCommandHandler : IRequestHandler<UpdateSeenStatusCommand, BaseResponse>
    {
        private readonly INotificationService _notificationService;

        public UpdateSeenStatusCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<BaseResponse> Handle(UpdateSeenStatusCommand request, CancellationToken cancellationToken)
        {
            //    await _notificationService.updateSeenStatus(request.Id);
            await _notificationService.RemoveNotification(request.Id);
            return new BaseResponse { Message = "Notification removed successfully", Status = 200 };
        }
    }
}