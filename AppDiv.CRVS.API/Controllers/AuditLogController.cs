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

        [HttpGet("SystemAudit")]
        public async Task<IActionResult> SystemAudit([FromQuery] SystemAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("WorkHistory")]
        public async Task<IActionResult> WorkHistoryAudit([FromQuery] WorkHistoryAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("LoginHistory")]
        public async Task<IActionResult> LoginHistoryAudit([FromQuery] LoginAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        
        [HttpGet("SystemAudit/Detail")]
        public async Task<IActionResult> SystemAuditDetail([FromQuery] SystemAuditDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

    }
}

