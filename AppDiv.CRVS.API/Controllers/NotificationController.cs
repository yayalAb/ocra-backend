
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

        public NotificationController(IHubContext < MessageHub, IMessageHubClient > messageHub)
        {
            _messageHub = messageHub;
        }

        [HttpPost]
        public async Task<IActionResult> GetNotification([FromBody] GetNotificationByGroupIdQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost("changeSeenStatus/{id}")]
        public async Task<IActionResult> changeSeenStatus(Guid id)
        {

            return Ok(await Mediator.Send(new UpdateSeenStatusCommand{Id = id}));
        }                                                                                                                                                                                                                                                                                                                                                                                                            
     
    }
}