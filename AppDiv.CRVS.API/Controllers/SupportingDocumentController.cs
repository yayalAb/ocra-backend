
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Notifications.Commands.UpdateSeenStatus;
using AppDiv.CRVS.Application.Notifications.Queries.GetNotificationByGroupId;
using Microsoft.AspNetCore.SignalR;
using AppDiv.CRVS.Infrastructure.Hub;
using AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.Create;
using AppDiv.CRVS.Application.Features.SupportingDocuments.Commands.DuplicateCheck;
// using AppDiv.CRVS.Utility.Hub;

namespace AppDiv.CRVS.API.Controllers
{
    public class SupportingDocumentController : ApiControllerBase
    {




        [HttpPost("saveSupportingDocuments")]
        public async Task<IActionResult> changeSeenStatus([FromBody] CreateSupportingDocumentsCommand command)
        {
            var createDocRes = await Mediator.Send(command);
            if (createDocRes.Success)
            {
                var duplicateCheckRes = await Mediator.Send(new DuplicateCheckCommand
                {
                    BiometricData = createDocRes.BiometricData,
                    SavedEvent = createDocRes.SavedEvent
                });
                duplicateCheckRes.Message = createDocRes.Message + "/n" + duplicateCheckRes.Message;


                return Ok(duplicateCheckRes);
            }
            else
            {
                return BadRequest(createDocRes);
            }

        }


    }
}