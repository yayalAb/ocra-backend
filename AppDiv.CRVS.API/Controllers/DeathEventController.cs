using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Delete;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Query;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeathEventController : ApiControllerBase
    {
        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> CreateDeathEvent(CreateDeathEventCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<DeathEventDTO>> Get([FromQuery] GetAllDeathEventQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<DeathEventDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetDeathEventByIdQuery(id));
        }


        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateDeathEventCommand command)
        {
            try
            {
                if (command.Id == id)
                {
                    var result = await Mediator.Send(command);
                    if (result.Success)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        return BadRequest(result);
                    }

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
        public async Task<ActionResult> DeleteDeathEvent(Guid id)
        {
            try
            {
                string result = string.Empty;
                result = await Mediator.Send(new DeleteDeathEventCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
    }
}