using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Queries.GetById;
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
    public class AdoptionEventController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<AdoptionEventController> _Ilog;
        public AdoptionEventController(ISender mediator, ILogger<AdoptionEventController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }
        [HttpPost("Create")]
        public async Task<ActionResult> CreateAdoption([FromBody] CreateAdoptionCommand command)
        {
            var res = await _mediator.Send(command);
            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }

        }
        [HttpPut]
        public async Task<IActionResult> update([FromBody] UpdateAdoptionCommand command)
        {
            var res = await _mediator.Send(command);
            if (res.Success)
            {
                return Ok(res);
            }
            else
            {
                return BadRequest(res);
            }

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdoptionEventById(Guid id)
        {

            return Ok(await _mediator.Send(new AdoptionEventGetByIdQuery { Id = id }));
        }

    }
}
