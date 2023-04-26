using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Delete;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.AllCountry;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressbyAdminstrativeLevel;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressById;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressByParent;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllRegion;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AddressController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<AddressController> _Ilog;
        public AddressController(ISender mediator, ILogger<AddressController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressDTO>> Get()
        {
            return await _mediator.Send(new GetAllAddressQuery());
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AddressDTO>> CreateAddress([FromBody] CreateAdderssCommand command)
        {
            _Ilog.LogCritical(command.Address.Code);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<AddressDTO> Get(Guid id)
        {
            return await _mediator.Send(new GetAddressByIdQuery(id));
        }
        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateaddressCommand command)
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
        public async Task<ActionResult> DeleteAddress(Guid id)
        {
            try
            {
                string result = string.Empty;
                result = await _mediator.Send(new DeleteAddressCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
        [HttpGet]
        [Route("GetByParent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressForLookupDTO>> GetByParent([FromQuery] Guid parentId)
        {
            return await _mediator.Send(new GetAddressByParntId { Id = parentId });
        }

        [HttpGet]
        [Route("administrativeLavel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressDTO>> administrativeLavel([FromQuery] Guid AdminLevelId)
        {
            return await _mediator.Send(new GetByAdminstrativeLevelQuery());
        }

        [HttpGet]
        [Route("Country")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<CountryDTO>> GetAllCountry()
        {
            return await _mediator.Send(new GetAllCountryQuery());
        }

        [HttpGet]
        [Route("Region")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<RegionDTO>> GetAllRegion()
        {
            return await _mediator.Send(new GetAllRegionQuery());
        }

    }
}