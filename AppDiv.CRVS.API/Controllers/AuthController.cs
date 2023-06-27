
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Auth.Login;
using AppDiv.CRVS.Application.Features.Auth.ForgotPassword;
using AppDiv.CRVS.Application.Features.Auth.ResetPassword;
using AppDiv.CRVS.Application.Features.Auth.ChangePassword;
using AppDiv.CRVS.Application.Features.Auth.UnlockUser;
using AppDiv.CRVS.Application.Features.Auth.YourTeam;
using AppDiv.CRVS.Domain;

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

        [HttpPut("Logout")]
        [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
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
        [HttpPost("changePassword")]
        // [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
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
        [HttpPost("unlockUser")]
        // [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> UnlockUser([FromBody] UnlockUserCommand command)
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

        [HttpGet("YourTeam")]
        public async Task<IActionResult> GetYourTeam([FromQuery] GetYourTeamQuery command)
        {
            return Ok(await Mediator.Send(command));

        }
    }
}