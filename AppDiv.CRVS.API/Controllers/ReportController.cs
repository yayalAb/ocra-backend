using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Features.Report.Commads;
using AppDiv.CRVS.Application.Features.Report.Commads.Delete;
using AppDiv.CRVS.Application.Features.Report.Commads.Update;
using AppDiv.CRVS.Application.Features.Report.Query;
using AppDiv.CRVS.Application.Features.SaveReports.Query;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ApiControllerBase
    {
        private readonly ISender _mediator;
        public ReportController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult> CreateGroup([FromBody] CreateReportCommad command)
        {
            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
        [HttpPost("GetReport")]
        public async Task<ActionResult> GetReport([FromBody] GetReportQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("GetReportList")]
        public async Task<ActionResult> GetReport([FromQuery] GetReportsList query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpPut("UpdateReport")]
        public async Task<ActionResult> UpdateReport([FromBody] UpdateReportCommand query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpGet("GetById")]
        public async Task<ActionResult> GetByName([FromQuery] GetReportDetailQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        }
        [HttpDelete("Delete")]
        public async Task<ActionResult> Delete([FromBody] DeleteReportCommand query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);

        }

        [HttpGet("GetColums")]
        public async Task<ActionResult> GetColums([FromQuery] GetReportColumsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);

        }
    }
}
