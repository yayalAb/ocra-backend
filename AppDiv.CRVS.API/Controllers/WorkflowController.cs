using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.WorkFlows.Query.GetAllWorkFlow;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Create;
using AppDiv.CRVS.Application.Features.WorkFlows.Query.GetWorkFlowById;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Update;
using AppDiv.CRVS.Application.Features.WorkFlows.Commands.Delete;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class WorkflowController : ApiControllerBase
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
        // WorkFlowSetting
        [CustomAuthorizeAttribute("WorkFlowSetting", "ReadAll")]
        
        public async Task<PaginatedList<GetAllWorkFlowDTO>> Get([FromQuery] GetAllWorkFlowQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [CustomAuthorizeAttribute("WorkFlowSetting", "Add")]

        public async Task<ActionResult<WorkflowDTO>> CreateGroup([FromBody] CreateWorkFlowCommand command, CancellationToken token)
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("WorkFlowSetting", "Update")]

        public async Task<WorkflowDTO> Get(Guid id)
        {
            return await _mediator.Send(new GetWorkFlowByIdQuery(id));
        }

        [HttpPut("Edit/{id}")]
        [CustomAuthorizeAttribute("WorkFlowSetting", "Update")]

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

        [HttpPut("StepEdit/{id}")]
        public async Task<ActionResult> StepUpdate(Guid id, [FromBody] UpdateStepCommand command)
        {
            try
            {
                if (command.Id == id)
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
        [CustomAuthorizeAttribute("WorkFlowSetting", "Delete")]

        public async Task<BaseResponse> DeleteLookup(DeleteWorkFlowCommad command)
        {
            try
            {

                return await _mediator.Send(command);

            }
            catch (Exception exp)
            {
                return new BaseResponse
                {
                    Message = exp.Message,
                    Success = false
                };
            }
        }
        [HttpDelete("StepDelete/{id}")]
        public async Task<BaseResponse> DeleteStep(Guid id)
        {
            try
            {
                return await _mediator.Send(new DeleteStepCommand { Id = id });
            }
            catch (Exception exp)
            {
                return new BaseResponse
                {
                    Message = exp.Message,
                    Success = false
                };
            }
        }




    }
}

