using AppDiv.CRVS.Application.Contracts.DTOs;

using AppDiv.CRVS.Application.Features.Lookups.Command.Update;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Groups.Query.GetAllGroup;
using AppDiv.CRVS.Application.Features.Groups.Commands.Create;
using AppDiv.CRVS.Application.Features.Groups.Query.GetGroupById;
using AppDiv.CRVS.Application.Features.Groups.Commands.Delete;
using AppDiv.CRVS.Application.Features.WorkFlows.Query.GetAllWorkFlow;
using AppDiv.CRVS.Application.Features.Lookups.Command.Create;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Create;
using AppDiv.CRVS.Application.Features.WorkFlows.Query.GetWorkFlowById;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Update;
using AppDiv.CRVS.Application.Features.Lookups.Command.Delete;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Delete;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkflowController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<WorkflowController> _Ilog;
        public WorkflowController(ISender mediator, ILogger<WorkflowController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<WorkflowDTO>> Get()
        {
            return await _mediator.Send(new GetAllWorkFlowQuery());
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<WorkflowDTO>> CreateGroup([FromBody] CreateWorkFlowCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<WorkflowDTO> Get(Guid id)
        {
            return await _mediator.Send(new GetWorkFlowByIdQuery(id));
        }

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateWorkFlowCommand command)
        {
            try
            {
                if (command.id == id)
                {
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }


        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteLookup([FromQuery] Guid id)
        {
            try
            {
                _Ilog.LogCritical(id.ToString());
                string result = string.Empty;
                result = await _mediator.Send(new DeleteWorkFlowCommad { Id = id });
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }




    }
}

