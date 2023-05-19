
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Courts.Commmands.Create;
using AppDiv.CRVS.Application.Features.Courts.Query.GetAllCourt;
using AppDiv.CRVS.Application.Features.Courts.Commmands.Delete;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.Courts.Commmands.Update;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupById;
using AppDiv.CRVS.Application.Features.Courts.Query.GetById;

namespace AppDiv.CRVS.API.Controllers
{
    public class CourtController : ApiControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateCourt([FromBody] CreateCourtCommand command)
        {

            return Ok(await Mediator.Send(command));
        }
        // [HttpPut]
        // public async Task<IActionResult> updateMarriageEvent([FromBody] UpdateMarriageEventCommand command)
        // {

        //     return Ok(await Mediator.Send(command));
        // }
        [HttpGet]
        public async Task<IActionResult> GetAllCourt()
        {

            return Ok(await Mediator.Send(new GetAllCourtQuery()));
        }

        [HttpDelete("Delete/{id}")]
        public async Task<Object> DeleteCourt(Guid id)
        {
            try
            {
                return await Mediator.Send(new DeleteCourtCommand { Id = id });
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


        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateCourtCommand command)
        {
            try
            {
                if (command.Id == id)
                {
                    var result = await Mediator.Send(command);
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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetById(Guid id)
        {
            return await Mediator.Send(new CourtGetByIdQuery(id));
        }
    }
}