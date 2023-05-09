using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Create;
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
    }
}
