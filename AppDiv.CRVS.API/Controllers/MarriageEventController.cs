
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

            return Ok(await Mediator.Send(command));
        }
        [HttpPut]
        public async Task<IActionResult> updateMarriageEvent([FromBody] UpdateMarriageEventCommand command)
        {

            return Ok(await Mediator.Send(command));
        }
        [HttpGet]
        public async Task<IActionResult> updateMarriageEvent([FromBody] GetMarriageEventByIdQuery query)
        {

            return Ok(await Mediator.Send(query));
        }
    }
}