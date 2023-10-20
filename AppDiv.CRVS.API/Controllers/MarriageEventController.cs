
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Create;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Query;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    public class MarriageEventController : ApiControllerBase
    {

        [HttpPost]
        [CustomAuthorizeAttribute("Marriage", "Add")]
        public async Task<IActionResult> createMarriageEvent([FromBody] CreateMarriageEventCommand command)
        {

            var res = await Mediator.Send(command);
            if (res.Success)
            {
                if (res.IsManualRegistration)
                {

                    await Mediator.Send(new GenerateCertificateQuery
                    {
                        Id = res.EventId,
                        CertificateSerialNumber = "manually-registered",
                        IsPrint = true,
                        CheckSerialNumber = false
                    });
                }
                return Ok(res);

            }
            else
            {
                return BadRequest(res);
            }
        }
        [HttpPut]
        [CustomAuthorizeAttribute("Marriage", "Update")]
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
        [CustomAuthorizeAttribute("Marriage", "Update")]
        public async Task<IActionResult> GetById(Guid id)
        {

            return Ok(await Mediator.Send(new GetMarriageEventByIdQuery { Id = id }));
        }
    }
}