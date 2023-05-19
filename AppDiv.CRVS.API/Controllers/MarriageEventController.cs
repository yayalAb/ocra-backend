
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Query;

namespace AppDiv.CRVS.API.Controllers
{
    public class MarriageEventController : ApiControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> createMarriageEvent([FromBody] CreateMarriageEventCommand command)
        {

            var res = await Mediator.Send(command);
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
        public async Task<IActionResult> updateMarriageEvent([FromBody] UpdateMarriageEventCommand command)
        {

            var res = await Mediator.Send(command);
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
        public async Task<IActionResult> updateMarriageEvent(Guid id)
        {

            return Ok(await Mediator.Send(new GetMarriageEventByIdQuery{Id = id}));
        }
    }
}