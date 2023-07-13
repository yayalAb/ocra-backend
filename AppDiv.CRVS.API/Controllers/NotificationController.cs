
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Notification.Commands.UpdateSeenStatus;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByGroupId;
using Microsoft.AspNetCore.SignalR;
using AppDiv.CRVS.Infrastructure.Hub;
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

        [HttpGet]
        public async Task<string> GetNotification([FromQuery] GetNotificationByGroupIdQuery query)
        {
            var res =  await Mediator.Send(query);
        //   Console.WriteLine($"Notification:----- {id}");    
          await _messageHub.Clients.All.SendNotification(res);
          
        //   await _messageHub.Clients.User(id).SendNotification(res);
            return "message sent ";
        }

        [HttpPost("changeSeenStatus/{id}")]
        public async Task<IActionResult> changeSeenStatus(Guid id)
        {

            return Ok(await Mediator.Send(new UpdateSeenStatusCommand { Id = id }));
        }

    }
}