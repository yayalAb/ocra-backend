
using AppDiv.CRVS.Application.Contracts.DTOs;

using MediatR;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Groups.Query.GetAllGroup;
using AppDiv.CRVS.Application.Features.Groups.Commands.Create;
using AppDiv.CRVS.Application.Features.Groups.Query.GetGroupById;
using AppDiv.CRVS.Application.Features.Groups.Commands.Delete;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.SaveReports.Commands.Delete;
using AppDiv.CRVS.Application.Features.SaveReports.Commands;
using AppDiv.CRVS.Application.Features.SaveReports.Query;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Features.SaveReports.Commands.Share;
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MyReportsController : ApiControllerBase
    {
        private readonly ISender _mediator;
        public MyReportsController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("MyReports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("MyReport", "ReadAll")]
        public async Task<List<MyReports>> Get()
        {
            return await _mediator.Send(new GetMyReports());
        }
        [HttpGet("GetReportById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("MyReport", "Update")]

        public async Task<object> GetMeyReportById([FromQuery] GetMyReportById query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Save")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [CustomAuthorizeAttribute("MyReport", "Create")]
        public async Task<ActionResult> SaveMyReport([FromBody] SaveReportCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }



        // [HttpPut("Edit/{id}")]
        // public async Task<ActionResult> Edit(Guid id, [FromBody] GroupUpdateCommand command)
        // {
        //     try
        //     {
        //         if (command.group.Id == id)
        //         {
        //             var result = await _mediator.Send(command);
        //             return Ok(result);
        //         }
        //         else
        //         {
        //             return BadRequest();
        //         }
        //     }
        //     catch (Exception exp)
        //     {
        //         return BadRequest(exp.Message);
        //     }
        // }



        [HttpPost("Share")]
        public async Task<ActionResult> Edit([FromBody] ShareMyReportCommands command)
        {
            try
            {

                var result = await _mediator.Send(command);
                return Ok(result);

            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

       [HttpDelete("Delete")]
        [CustomAuthorizeAttribute("MyReport", "Delete")]

        public async Task<IActionResult> DeleteMyReport([FromBody] DeleteMyReportCommand command)
        {
            var res = await _mediator.Send(command);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }



    }
}