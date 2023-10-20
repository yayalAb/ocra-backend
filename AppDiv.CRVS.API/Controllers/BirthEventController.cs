using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Create;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Delete;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    // Birth event controller
    [ApiController]
    [Route("api/[controller]")]
    public class BirthEventController : ApiControllerBase
    {
        // Create birth event
        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        [CustomAuthorizeAttribute("Birht", "Add")]
        public async Task<ActionResult> CreateBirthEvent(CreateBirthEventCommand command)
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

        // Get birth event by id
        [HttpGet("{id}")]
        [CustomAuthorizeAttribute("Birht", "Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<BirthEventDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetBirthEventByIdQuery(id));
        }

        // Edit birth event
        [HttpPut("Edit/{id}")]
        [CustomAuthorizeAttribute("Birht", "Update")]
        
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateBirthEventCommand command)
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

        // Delete birth event
        [HttpDelete("Delete/{id}")]
        [CustomAuthorizeAttribute("Birht", "Delete")]
        public async Task<ActionResult> DeleteBirthEvent(Guid id)
        {
            try
            {
                string result = string.Empty;
                result = await Mediator.Send(new DeleteBirthEventCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
    }
}