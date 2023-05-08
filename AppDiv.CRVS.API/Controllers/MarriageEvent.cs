
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;


namespace AppDiv.CRVS.API.Controllers
{
    public class MarriageEventController : ApiControllerBase
    {

        [HttpPost()]
        public async Task<IActionResult> createMarriageEvent([FromBody] CreateMarriageEventCommand command)
        {
    
            return Ok(await Mediator.Send(command));
        }

    }
}