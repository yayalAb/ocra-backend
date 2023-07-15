using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    public class AuditLogController : ApiControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAuditLogQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("GetDetails")]
        public async Task<IActionResult> GetDetails([FromQuery] GetAuditLogDetailQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        // [HttpGet("AuditLog")]
        // public async Task<IActionResult> AuditLog([FromQuery] GetAllLogHistoryQuery query)
        // {

        //     return Ok(await Mediator.Send(query));
        // }

    }
}

