using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Features.EventImport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventImportController : ApiControllerBase
    {

        public EventImportController()
        {
        }

        [HttpPost]
        [CustomAuthorizeAttribute("importEvents", "Add")]
        public async Task<IActionResult> ImportEvents([FromForm] EventImportCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


    }
}