using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Command.Create;
using AppDiv.CRVS.Application.Features.Customers.Command.Delete;
using AppDiv.CRVS.Application.Features.Customers.Command.Update;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.Lookups.Command.Create;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupById;
using AppDiv.CRVS.Application.Features.Lookups.Command.Delete;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Features.Lookups.Command.Update;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetListOfLookup;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByKey;
using AppDiv.CRVS.Application.Features.Groups.Query.GetAllGroup;
using AppDiv.CRVS.Application.Features.Groups.Commands.Create;
using AppDiv.CRVS.Application.Features.Groups.Query.GetGroupById;
using AppDiv.CRVS.Application.Features.Groups.Commands.Delete;
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GroupController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<GroupController> _Ilog;
        public GroupController(ISender mediator, ILogger<GroupController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<FetchGroupDTO>> Get()
        {
            return await _mediator.Send(new GetAllGroupQuery());
        }
        [HttpGet("lookup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<DropDownDto>> GetDropdown()
        {
            return await _mediator.Send(new GetDropDownGroups());
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<GroupDTO>> CreateGroup([FromBody] CreateGroupCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<GroupDTO> Get(Guid id)
        {
            return await _mediator.Send(new GetGroupbyId(id));
        }

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] GroupUpdateCommand command)
        {
            try
            {
                if (command.group.Id == id)
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


        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeleteLookup(Guid id)
        {
            try
            {
                string result = string.Empty;
                result = await _mediator.Send(new DeleteGroupCommands { Id = id });
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }




    }
}