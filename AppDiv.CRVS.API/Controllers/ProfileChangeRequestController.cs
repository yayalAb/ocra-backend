
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.ProfileChangeRequests.Commands.Approve;
using AppDiv.CRVS.Application.Features.ProfileChangeRequests.Query.GetForApproval;

namespace AppDiv.CRVS.API.Controllers
{
    public class ProfileChangeRequestController : ApiControllerBase
    {
        [HttpPost("Approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ApproveProfileChangeRequest([FromBody] ApproveProfileChangeRequestCommand command)
        {
            var res = await Mediator.Send(command);
            if (res.Success)
            {
                return Ok(res);

            }
            return BadRequest(res);

        }
        [HttpGet("getProfileChangeForApproval")]
        public async Task<ActionResult> getUserDataForApproval([FromQuery] GetProfileChangeForApproval query)
        {
            try
            {
                var result = await Mediator.Send(query);

                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
    }
}