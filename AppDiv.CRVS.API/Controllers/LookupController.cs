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
using AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByKeyForDropDown;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupByParentId;
using AppDiv.CRVS.Application.Features.Lookups.Command.Import;
using AppDiv.CRVS.Application.Features.Lookups.Query.Validation;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetDefualtAddress;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LookupController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<LookupController> _Ilog;

        public LookupController(ISender mediator, ILogger<LookupController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<List<LookupForGridDTO>> Get([FromQuery] GetAllLookupQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpGet("GetLastModified")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetChanged([FromQuery] GetLastModifiedAddressAndLookupsQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<LookupDTO>> CreateLookup([FromBody] CreateLookupCommand command, CancellationToken token)
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


        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteLookup([FromBody] DeleteLookupCommand command)
        {
            var res = await _mediator.Send(command);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);


        }

        [HttpGet]
        [Route("key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<LookupByKeyDTO>> GetByKey([FromQuery] GetLookupByKeyQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpGet]
        [Route("ByParentId/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<LookupByKeyDTO>> GetByKey(Guid id)
        {
            return await _mediator.Send(new GetLookupByParentIdQuery { Id = id });
        }


        [HttpGet("List")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Get([FromQuery] string[] keys)
        {
            return await _mediator.Send(new GetListOfLookupQuery { list = keys });
        }

        [HttpPost("GetListofLookup")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<object>> GetListLookup([FromBody] string[] command, CancellationToken token)
        {
            return await _mediator.Send(new GetListOfLookupQuery { list = command });
        }

        [HttpGet("LookupforDropdown")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<object>> LookupDropdown([FromQuery] GetLookupByKeyForDropDownQuery query)
        {
            return await _mediator.Send(query);
        }
        [HttpPost("Import")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<object>> ImportLookup([FromBody] ImportLookupCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("IsValid")]
        public async Task<ActionResult<object>> validate([FromBody] ValidatLookupNameQuery command)
        {
            return await _mediator.Send(command);
        }


    }
}
