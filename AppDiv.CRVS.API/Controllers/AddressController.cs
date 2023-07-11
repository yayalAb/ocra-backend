using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Delete;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.MergeAndSplitCommands;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.AllCountry;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressbyAdminstrativeLevel;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressById;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressByParent;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddressInfoByParent;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllKebele;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllRegion;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllWoreda;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllZone;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetDefualtAddress;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<List<AddressDTO>> Get([FromQuery] GetAllAddressQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AddressDTO>> CreateAddress([FromBody] CreateAdderssCommand command)
        {
            _Ilog.LogCritical(command.Address.Code);

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

        [HttpPost("Merge")]
        public async Task<ActionResult<AddressDTO>> MerigeAndSplit([FromBody] MergeAndSplitCommand command)
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




        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Get(Guid id)
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
        public async Task<Object> DeleteAddress(Guid id)
        {
            try
            {
                var response = await _mediator.Send(new DeleteAddressCommand { Id = id });
                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
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
        [Route("GetByParent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressForLookupDTO>> GetByParent([FromQuery] Guid parentId)
        {
            return await _mediator.Send(new GetAddressByParntId { Id = parentId });
        }
        [HttpGet]
        [Route("GetDetailByParent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressForMergeDTO>> GetAllByParent([FromQuery] Guid parentId)
        {
            return await _mediator.Send(new GetlaaAddressInfoByParentQuery { Id = parentId });
        }


        [HttpGet]
        [Route("DefualtAddress")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressForLookupDTO>> DefualtAddress([FromQuery] bool IsRegion)
        {
            return await _mediator.Send(new DefualtAddressQuery { IsRegion = IsRegion });
        }
        [HttpGet]
        [Route("administrativeLavel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AddressDTO>> administrativeLavel([FromQuery] Guid AdminLevelId)
        {
            return await _mediator.Send(new GetByAdminstrativeLevelQuery());
        }
        // [Authorize]
        [HttpGet]
        [Route("Country")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<CountryDTO>> GetAllCountry([FromQuery] GetAllCountryQuery query)
        {
            return await _mediator.Send(query);
        }
        // [Authorize]
        [HttpGet]
        [Route("Region")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<RegionDTO>> GetAllRegion([FromQuery] GetAllRegionQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet]
        [Route("Zone")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<ZoneDTO>> GetAllZone([FromQuery] GetAllZoneQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet]
        [Route("Woreda")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<WoredaDTO>> GetAllWoreda([FromQuery] GetAllWoredaQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet]
        [Route("Kebele")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<KebeleDTO>> GetAllKebele([FromQuery] GetAllKebeleQuery query)
        {
            return await _mediator.Send(query);
        }

    }
}