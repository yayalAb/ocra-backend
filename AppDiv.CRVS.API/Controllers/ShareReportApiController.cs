using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Features.ShareReportApi.commands.create;
using AppDiv.CRVS.Application.Features.ShareReportApi.commands.delete;
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
        public async Task<ActionResult> CreateGroup([FromBody] CreateReportApiCommands command)
        {
            var result = await _mediator.Send(command);

                return Ok(result);
        }
        [HttpPut("UpdateReportAPi")]
        public async Task<ActionResult> UpdateReport([FromBody] updateSharedReportApi query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
       
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete([FromBody] DeleteReportApi query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);

        }
      [HttpGet("GetAllSharedReportes")]
        public async Task<ActionResult> GetReport([FromQuery] getAllSharedReportQueryHandler query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("GetSharedReportById")]
        public async Task<ActionResult> GetReport([FromQuery] getSharedReportById query)
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
    }
}