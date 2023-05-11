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
    public class AdoptionEventController : ControllerBase
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
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> update([FromBody] UpdateAdoptionCommand command)
        {
            return Ok(await _mediator.Send(command));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdoptionEventById(Guid id)
        {

            return Ok(await _mediator.Send(new AdoptionEventGetByIdQuery { Id = id }));
        }

    }
}
