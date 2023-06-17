using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Plans.Command.Create;
using AppDiv.CRVS.Application.Features.Plans.Command.Delete;
using AppDiv.CRVS.Application.Features.Plans.Command.Update;
using AppDiv.CRVS.Application.Features.Plans.Query;
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
    public class PlanController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<PlanController> _Ilog;
        public PlanController(ISender mediator, ILogger<PlanController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<PlanDTO>> Get([FromQuery] GetAllPlanQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PlanDTO>> CreatePlan([FromBody] CreatePlanCommand command)
        {
            // _Ilog.LogCritical(command.Plan);

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
            return await _mediator.Send(new GetPlanByIdQuery { Id = id });
        }
        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdatePlanCommand command)
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
        public async Task<Object> DeletePlan(Guid id)
        {
            try
            {
                return await _mediator.Send(new DeletePlanCommand(id));
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
        // public async Task<List<PlanForLookupDTO>> GetByParent([FromQuery] Guid parentId)
        // {
        //     return await _mediator.Send(new GetPlanByParntId { Id = parentId });
        // }

        // [HttpGet]
        // [Route("DefualtPlan")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<List<PlanForLookupDTO>> DefualtPlan([FromQuery] bool IsRegion)
        // {
        //     return await _mediator.Send(new DefualtPlanQuery { IsRegion = IsRegion });
        // }
        // [HttpGet]
        // [Route("administrativeLavel")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<List<PlanDTO>> administrativeLavel([FromQuery] Guid AdminLevelId)
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