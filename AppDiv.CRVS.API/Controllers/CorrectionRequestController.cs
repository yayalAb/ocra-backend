using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Commands;
using MediatR;
using Microsoft.AspNetCore.Cors;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CorrectionRequestController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<CorrectionRequestController> _Ilog;
        public CorrectionRequestController(ISender mediator, ILogger<CorrectionRequestController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }
        [HttpPost]
        public async Task<IActionResult> CorrectionREquest([FromBody] CreateCorrectionRequest command)
        {

            return Ok(await Mediator.Send(command));
        }
    }
}