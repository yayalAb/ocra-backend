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
using AppDiv.CRVS.Application.Common;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LookupController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<LookupController> _Ilog;
        public LookupController(ISender mediator, ILogger<LookupController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<LookupForGridDTO>> Get([FromQuery] GetAllLookupQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<LookupDTO>> CreateLookup([FromBody] CreateLookupCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<LookupDTO> Get(Guid id)
        {
            return await _mediator.Send(new GetLookupByIdQuery(id));
        }

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateLookupCommand command)
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


        [HttpDelete("Delete/{id}")]
        public async Task<BaseResponse> DeleteLookup(Guid id)
        {
            try
            {
                string result = string.Empty;
                return await _mediator.Send(new DeleteLookupCommand { Id = id });
            }
            catch (Exception exp)
            {
                var res = new BaseResponse
                {
                    Success = false,
                    Message = exp.Message
                };
                return res;
            }
        }

        [HttpGet]
        [Route("key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<LookupByKeyDTO>> GetByKey([FromQuery] string Key)
        {
            return await _mediator.Send(new GetLookupByKeyQuery { Key = Key });
        }


        [HttpGet("List")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Get([FromQuery] string[] keys)
        {
            _Ilog.LogCritical(keys[0]);
            return await _mediator.Send(new GetListOfLookupQuery { list = keys });
        }

        [HttpPost("GetListofLookup")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<object>> GetListLookup([FromBody] string[] command, CancellationToken token)
        {
            return await _mediator.Send(new GetListOfLookupQuery { list = command });
        }

    }
}