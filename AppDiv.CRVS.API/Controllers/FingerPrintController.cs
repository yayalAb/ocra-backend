using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Features.Fingerprint.commands;
using AppDiv.CRVS.Application.Features.Fingerprint.commands.Create;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
// 
namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class FingerPrintController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<GroupController> _Ilog;
        public FingerPrintController(ISender mediator, ILogger<GroupController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] CreateFingerprint command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);

            return Ok(result);
        }
        [HttpPost("Identify")]
        public async Task<ActionResult> Identify([FromBody] IdentifayCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);

            return Ok(result);
        }
        [HttpPost("Verify")]
        public async Task<ActionResult> Verify([FromBody] VerifayCommands command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            return Ok(result);
        }
    }
}