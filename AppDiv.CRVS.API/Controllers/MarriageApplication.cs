
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageApplications.Query;

namespace AppDiv.CRVS.API.Controllers
{
    public class MarriageApplicationController : ApiControllerBase
    {

        [HttpPost("MarriageApplication")]
        public async Task<IActionResult> createMarriageApplication([FromBody] CreateMarriageApplicationCommand command)
        {
    
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("MarriageApplication")]
        public async Task<IActionResult> UpdateMarriageApplication([FromBody] UpdateMarriageApplicationCommand command)
        {
    
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("MarriageApplication")]
        public async Task<IActionResult> GetAllMarriageApplications([FromQuery] GetAllMarriageApplicationsQuery query)
        {
    
            return Ok(await Mediator.Send(query));
        }
        [HttpGet("MarriageApplication/{id}")]
        public async Task<IActionResult> GetMarriageApplicationById(Guid id)
        {
    
            return Ok(await Mediator.Send(new GetMarriageApplicationByIdQuery{Id = id}));
        }
    }
}