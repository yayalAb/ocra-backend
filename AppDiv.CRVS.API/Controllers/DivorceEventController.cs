
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Query;

namespace AppDiv.CRVS.API.Controllers
{
    public class DivorceEventController : ApiControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> createDivorceEvent([FromBody] CreateDivorceEventCommand command)
        {

            return Ok(await Mediator.Send(command));
        }
        [HttpPut]
        public async Task<IActionResult> updateDivorceEvent([FromBody] UpdateDivorceEventCommand command)
        {

            return Ok(await Mediator.Send(command));
        }
        [HttpGet]
        public async Task<IActionResult> updateDivorceEvent([FromBody] GetDivorceEventByIdQuery query)
        {

            return Ok(await Mediator.Send(query));
            
        }
    }
}