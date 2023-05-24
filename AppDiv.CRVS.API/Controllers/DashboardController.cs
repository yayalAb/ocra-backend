using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DashboardController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<DashboardController> _Ilog;
        public DashboardController(ISender mediator, ILogger<DashboardController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Get([FromQuery] DashboardQuery query)
        {
            return await _mediator.Send(query);
        }



    }
}