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
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
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
        public async Task<BaseResponse> Get([FromQuery] AuthenticatCommand query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet("AuthenticationRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<BaseResponse> AuthenticationRequest([FromQuery] AuthenticationRequestCommad query)
        {
            return await _mediator.Send(query);
        }

        [HttpGet("AuthenticationRequestList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<AuthenticationRequestListDTO>> AuthenticationRequests()
        {
            return await _mediator.Send(new GetAuthentcationRequestList());
        }



    }
}
