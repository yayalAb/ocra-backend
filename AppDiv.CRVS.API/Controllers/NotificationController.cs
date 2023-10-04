
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Notification.Commands.UpdateSeenStatus;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByGroupId;
using Microsoft.AspNetCore.SignalR;
using AppDiv.CRVS.Infrastructure.Hub;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByTransactionId;
// using AppDiv.CRVS.Utility.Hub;

namespace AppDiv.CRVS.API.Controllers
{
    public class NotificationController : ApiControllerBase
    {
        private readonly IHubContext<MessageHub, IMessageHubClient> _messageHub;

        public NotificationController(IHubContext<MessageHub, IMessageHubClient> messageHub)
        {
            _messageHub = messageHub;
        }

        [HttpPost]
        public async Task<IActionResult> GetNotification([FromBody] GetNotificationByGroupIdQuery query)
        {
            var res =  await Mediator.Send(query);
            return Ok(res);
        }

        [HttpPost("changeSeenStatus/{id}")]
        public async Task<IActionResult> changeSeenStatus(Guid id)
        {

            return Ok(await Mediator.Send(new UpdateSeenStatusCommand { Id = id }));
        }
        [HttpPost("test")]
        public async Task<IActionResult> test([FromBody] GetNotificationByGroupIdQuery query)
        {
              var res =  await Mediator.Send(query);
             await _messageHub.Clients.Group(res.FirstOrDefault().GroupId.ToString()).NewNotification(res.First());


            return Ok("notification sent");
        }

        [HttpGet("Data")]
        public async Task<NotificationData?> GetNotificationData([FromQuery] Guid? requestId, [FromQuery] Guid? transactionId)
        {
            if (requestId != null)
            {
                return await Mediator.Send(new GetNotificationByIdQuery { Id = (Guid)requestId });
            }
            else if(transactionId != null)
            {
                return await Mediator.Send(new GetNotificationByTransactionIdQuery { Id = (Guid)transactionId });
            }
            return null;
        }

    }
}