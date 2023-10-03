
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Auth.Login;
using AppDiv.CRVS.Application.Features.Auth.ForgotPassword;
using AppDiv.CRVS.Application.Features.Auth.ResetPassword;
using AppDiv.CRVS.Application.Features.Auth.ChangePassword;
using AppDiv.CRVS.Application.Features.Auth.UnlockUser;
using AppDiv.CRVS.Application.Features.Auth.YourTeam;
using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Application.Features.Auth.VerifyOtp;
using Microsoft.AspNetCore.Authorization;
using MediatR;

namespace AppDiv.CRVS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ISender Mediator;

        public AuthController(ISender _mediator)
        {
            Mediator = _mediator;

        }

        [HttpPost("Login")]
        [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> Login([FromBody] AuthCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPut("Logout")]
        [ProducesDefaultResponseType(typeof(AuthResponseDTO))]
        public async Task<IActionResult> Logout()
        {
            return Ok(await Mediator.Send(new LogoutCommand()));
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
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var verifyRes = await Mediator.Send(command);
            if (verifyRes.Success)
            {
                return Ok(await Mediator.Send(new AuthCommand
                {
                    SendOTP = false,
                    FromVerifyOtpCmd = true,
                    UserId = verifyRes.UserId,
                    Roles = verifyRes.Roles
                }));
            }
            else
            {
                return BadRequest(verifyRes);
            }
        }
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}