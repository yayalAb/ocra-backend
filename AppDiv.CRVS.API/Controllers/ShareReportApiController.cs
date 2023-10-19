using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Features.ShareReportApi.commands.create;
using AppDiv.CRVS.Application.Features.ShareReportApi.commands.delete;
using AppDiv.CRVS.Application.Features.ShareReportApi.commands.RefrashToken;
using AppDiv.CRVS.Application.Features.ShareReportApi.commands.update;
using AppDiv.CRVS.Application.Features.ShareReportApi.Querys;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.API.Controllers
{

    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShareReportApiController : ApiControllerBase
    {
        private readonly ISender _mediator;
        public ShareReportApiController(ISender mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Create")]
        [CustomAuthorizeAttribute("Integration", "Add")]

        public async Task<ActionResult> CreateGroup([FromBody] CreateReportApiCommands command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }
        [HttpPut("UpdateReportAPi")]
        [CustomAuthorizeAttribute("Integration", "Update")]

        public async Task<ActionResult> UpdateReport([FromBody] updateSharedReportApi query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }

        [HttpDelete("Delete")]
        [CustomAuthorizeAttribute("Integration", "Delete")]

        public async Task<ActionResult> Delete([FromBody] DeleteReportApi query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);

        }
        [HttpGet("GetAllSharedReportes")]
        [CustomAuthorizeAttribute("Integration", "ReadAll")]

        public async Task<ActionResult> GetReport([FromQuery] getAllSharedReportQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("GetSharedReportById")]
        [CustomAuthorizeAttribute("Integration", "ReadSingle")]

        public async Task<ActionResult> GetReportById([FromQuery] getSharedReportById query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("GetSharedReportData")]
        public async Task<ActionResult> GetReport([FromQuery] GetSharedReportData query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("RefrashToken")]
        [CustomAuthorizeAttribute("Integration", "ReadSingle")]

        public async Task<ActionResult> RefrashToken([FromQuery] RefrashTokenCommand query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("Deactivate")]
        [CustomAuthorizeAttribute("Integration", "ReadAll")]

        public async Task<ActionResult> DeactivateApi([FromQuery] DeactivateApi query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }



    }
}