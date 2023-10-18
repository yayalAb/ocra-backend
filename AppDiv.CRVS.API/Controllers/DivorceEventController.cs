
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Create;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Query;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    public class DivorceEventController : ApiControllerBase
    {

        [HttpPost]
        [CustomAuthorizeAttribute("Devorce", "Add")]

        public async Task<IActionResult> createDivorceEvent([FromBody] CreateDivorceEventCommand command)
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
        [CustomAuthorizeAttribute("Devorce", "Update")]
        public async Task<IActionResult> updateDivorceEvent([FromBody] UpdateDivorceEventCommand command)
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
        [CustomAuthorizeAttribute("Devorce", "Update")]

        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await Mediator.Send(new GetDivorceEventByIdQuery { Id = id }));
        }
        [HttpGet]
        [Route("getWife/{id}")]
        public async Task<IActionResult> getWives(Guid id)
        {
            return Ok(await Mediator.Send(new GetWivesQuery { HusbandId = id }));
        }
    }
}