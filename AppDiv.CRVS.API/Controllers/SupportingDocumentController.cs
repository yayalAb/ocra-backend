
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Notification.Commands.UpdateSeenStatus;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByGroupId;
using Microsoft.AspNetCore.SignalR;
using AppDiv.CRVS.Infrastructure.Hub;
using AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create;
// using AppDiv.CRVS.Utility.Hub;

namespace AppDiv.CRVS.API.Controllers
{
    public class SupportingDocumentController : ApiControllerBase
    {




        [HttpPost("saveSupportingDocuments")]
        public async Task<IActionResult> changeSeenStatus([FromBody] CreateSupportingDocumentsCommand command)
        {
            var res = await Mediator.Send(command);
            if (res.Success)
            {

                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }

        }


    }
}