using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Certificates.Query;
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
        [CustomAuthorizeAttribute("Death", "Add")]
        public async Task<ActionResult> CreateDeathEvent(CreateDeathEventCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                if (result.Success)
                {
                    if (result.IsManualRegistration)
                    {

                        await Mediator.Send(new GenerateCertificateQuery
                        {
                            Id = result.EventId,
                            CertificateSerialNumber = "manually-registered",
                            IsPrint = true,
                            CheckSerialNumber = false
                        });
                    }
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
        [CustomAuthorizeAttribute("Death", "ReadAll")]

        public async Task<PaginatedList<DeathEventDTO>> Get([FromQuery] GetAllDeathEventQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("Death", "Update")]

        public async Task<DeathEventDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetDeathEventByIdQuery(id));
        }


        [HttpPut("Edit/{id}")]
        [CustomAuthorizeAttribute("Death", "Update")]
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
        [CustomAuthorizeAttribute("Death", "Delete")]

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