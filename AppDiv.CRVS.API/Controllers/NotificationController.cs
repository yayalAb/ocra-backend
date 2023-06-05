
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Notification.Commands.UpdateSeenStatus;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByGroupId;

namespace AppDiv.CRVS.API.Controllers
{
    public class NotificationController : ApiControllerBase
    {

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