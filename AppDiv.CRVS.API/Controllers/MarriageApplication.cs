
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Auth.Login;
using AppDiv.CRVS.Application.Features.Auth.ForgotPassword;
using AppDiv.CRVS.Application.Features.Auth.ResetPassword;
using AppDiv.CRVS.Application.Features.Auth.ChangePassword;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;

namespace AppDiv.CRVS.API.Controllers
{
    public class MarriageApplicationController : ApiControllerBase
    {

        [HttpPost("MarriageApplication")]
        public async Task<IActionResult> createMarriageApplication([FromBody] CreateMarriageApplicationCommand command)
        {
    
            return Ok(await Mediator.Send(command));
        }
    }
}