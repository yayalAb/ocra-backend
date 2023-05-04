
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Auth.Login;
using AppDiv.CRVS.Application.Features.Auth.ForgotPassword;
using AppDiv.CRVS.Application.Features.Auth.ResetPassword;
using AppDiv.CRVS.Application.Features.Auth.ChangePassword;

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
        [HttpPost("resetPassword")]
        // [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("changePassword")]
        // [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}