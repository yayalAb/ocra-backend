using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Features.LoginHistorys.LogHistory;
using AppDiv.CRVS.Application.Features.LoginHistorys.Query;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    public class LoginHistoryController : ApiControllerBase
    {
        [HttpGet]
        [CustomAuthorizeAttribute("userAuditlog", "ReadAll")]

        public async Task<IActionResult> GetAllHistory([FromQuery] GetAllLoginQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        [HttpGet("GetUserLoging")]
        [CustomAuthorizeAttribute("User", "ReadSingle")]

        public async Task<IActionResult> GetUserHistory([FromQuery] GetUserLoginHistory query)
        {

            return Ok(await Mediator.Send(query));
        }

        [HttpGet("AuditLog")]
        // ??
        public async Task<IActionResult> AuditLog([FromQuery] GetAllLogHistoryQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

    }
}

