using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    public class AuditLogController : ApiControllerBase
    {
        [HttpGet("GetAll")]
        [CustomAuthorizeAttribute("auditLog", "ReadAll")]
        public async Task<IActionResult> GetAll([FromQuery] GetAllAuditLogQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("GetDetails")]
        [CustomAuthorizeAttribute("auditLog", "ReadSingle")]
        public async Task<IActionResult> GetDetails([FromQuery] GetAuditLogDetailQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        [HttpGet("SystemAudit")]
        [CustomAuthorizeAttribute("systemAuditlog", "ReadAll")]

        public async Task<IActionResult> SystemAudit([FromQuery] SystemAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("WorkHistory")]
        [CustomAuthorizeAttribute("workHistoryAuditLog", "ReadAll")]

        public async Task<IActionResult> WorkHistoryAudit([FromQuery] WorkHistoryAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpGet("LoginHistory")]
        [CustomAuthorizeAttribute("userAuditlog", "ReadAll")]

        public async Task<IActionResult> LoginHistoryAudit([FromQuery] LoginAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        
        [HttpGet("EventAudit")]
        [CustomAuthorizeAttribute("eventAuditLog", "ReadAll")]

        public async Task<IActionResult> EventAudit([FromQuery] EventAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        
        [HttpGet("TransactionAudit")]
        [CustomAuthorizeAttribute("transactionAudit", "ReadAll")]

        public async Task<IActionResult> TransactionAudit([FromQuery] TransactionAuditQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
        
        [HttpGet("SystemAudit/Detail")]
        [CustomAuthorizeAttribute("systemAuditlog", "ReadSingle")]

        public async Task<IActionResult> SystemAuditDetail([FromQuery] SystemAuditDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

    }
}

