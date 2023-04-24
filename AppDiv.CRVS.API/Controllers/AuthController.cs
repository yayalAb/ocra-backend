
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Auth.Login;
using AppDiv.CRVS.Application.Features.Auth.ForgotPassword;

namespace AppDiv.CRVS.API.Controllers
{
    public class AuthController : ApiControllerBase
    {

        [HttpPost("Login")]
        [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> Login([FromBody] AuthCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        
        [HttpPost("forgotPassword")]
        // [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}