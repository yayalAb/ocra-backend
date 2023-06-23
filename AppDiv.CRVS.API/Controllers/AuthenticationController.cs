using System.Security.AccessControl;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Authentication.Commands;
using AppDiv.CRVS.Application.Features.Authentication.Querys;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Approve;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
// using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AppDiv.CRVS.API.Controllers
{
    // [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]



    public class AuthenticationController : ApiControllerBase
    {
        private readonly ISender _mediator;
        public AuthenticationController(ISender mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get([FromQuery] AuthenticatCommand query)
        {
            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("AuthenticationRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> AuthenticationRequest([FromQuery] AuthenticationRequestCommad query)
        {
            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }

        [HttpGet("AuthenticationRequestList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> AuthenticationRequests([FromQuery] GetAuthentcationRequestList query)
        {
            return await _mediator.Send(query);
        }



    }
}
