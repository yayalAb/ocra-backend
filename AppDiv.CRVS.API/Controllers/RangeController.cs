using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Ranges.Command.Create;
using AppDiv.CRVS.Application.Features.Ranges.Command.Delete;
using AppDiv.CRVS.Application.Features.Ranges.Command.Update;
using AppDiv.CRVS.Application.Features.Ranges.Query;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace AppDiv.CRVS.API.Controllers
{
    // [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RangeController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<RangeController> _Ilog;
        public RangeController(ISender mediator, ILogger<RangeController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<RangeDTO>> Get([FromQuery] GetAllRangeQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<RangeDTO>> CreateRange([FromBody] CreateRangeCommand command)
        {
            // _Ilog.LogCritical(command.Range);

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
            return await _mediator.Send(new GetRangeByIdQuery { Id = id });
        }
        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateRangeCommand command)
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
        public async Task<Object> DeleteRange([FromBody] DeleteRangeCommand commads)
        {
            try
            {
                return await _mediator.Send(commads);
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
        // [HttpGet]
        // [Route("GetByParent")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<List<RangeForLookupDTO>> GetByParent([FromQuery] Guid parentId)
        // {
        //     return await _mediator.Send(new GetRangeByParntId { Id = parentId });
        // }

        // [HttpGet]
        // [Route("DefualtRange")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<List<RangeForLookupDTO>> DefualtRange([FromQuery] bool IsRegion)
        // {
        //     return await _mediator.Send(new DefualtRangeQuery { IsRegion = IsRegion });
        // }
        // [HttpGet]
        // [Route("administrativeLavel")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<List<RangeDTO>> administrativeLavel([FromQuery] Guid AdminLevelId)
        // {
        //     return await _mediator.Send(new GetByAdminstrativeLevelQuery());
        // }

        // [HttpGet]
        // [Route("Country")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<PaginatedList<CountryDTO>> GetAllCountry([FromQuery] GetAllCountryQuery query)
        // {
        //     return await _mediator.Send(query);
        // }

        // [HttpGet]
        // [Route("Region")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<PaginatedList<RegionDTO>> GetAllRegion([FromQuery] GetAllRegionQuery query)
        // {
        //     return await _mediator.Send(query);
        // }

        // [HttpGet]
        // [Route("Zone")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<PaginatedList<ZoneDTO>> GetAllZone([FromQuery] GetAllZoneQuery query)
        // {
        //     return await _mediator.Send(query);
        // }

        // [HttpGet]
        // [Route("Woreda")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<PaginatedList<WoredaDTO>> GetAllWoreda([FromQuery] GetAllWoredaQuery query)
        // {
        //     return await _mediator.Send(query);
        // }

        // [HttpGet]
        // [Route("Kebele")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<PaginatedList<KebeleDTO>> GetAllKebele([FromQuery] GetAllKebeleQuery query)
        // {
        //     return await _mediator.Send(query);
        // }

    }
}