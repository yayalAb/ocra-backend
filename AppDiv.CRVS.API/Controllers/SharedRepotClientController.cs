using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Features.ShareReportApi.Querys;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace AppDiv.CRVS.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class SharedRepotClientController : ControllerBase
    {
        private readonly ISender _mediator;

        public SharedRepotClientController(ISender mediator)
        {
            _mediator = mediator;

        }

        [HttpPost("GetReportData")]
        public async Task<ActionResult> GetReport([FromBody] GetSharedReportClient query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);

        } 

        
}
}