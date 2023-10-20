using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Command.Create;
using AppDiv.CRVS.Application.Features.Customers.Command.Delete;
using AppDiv.CRVS.Application.Features.Customers.Command.Update;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.Settings.Commands.create;
using AppDiv.CRVS.Application.Features.Settings.Commands.Update;
using AppDiv.CRVS.Application.Features.Settings.create;
using AppDiv.CRVS.Application.Features.Settings.Query.GetAllSettings;
using AppDiv.CRVS.Application.Features.Settings.Query.GetSettingByKey;
using AppDiv.CRVS.Application.Features.Settings.Query.GetSettingsById;
using AppDiv.CRVS.Domain.Entities;

using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SettingController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<SettingController> _Ilog;
        public SettingController(ISender mediator, ILogger<SettingController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }


        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("ConfigurationSetting", "Update")]

        public async Task<List<SettingDTO>> Get([FromQuery] GetAllSettingQuery query)
        {
            return await _mediator.Send(query);
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [CustomAuthorizeAttribute("ConfigurationSetting", "Update")]

        public async Task<ActionResult<SettingDTO>> CreateSetting([FromBody] createSettingCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Setting> Get(Guid id)
        {
            return await _mediator.Send(new GetSettingByIdQuery { Id = id });
        }

        [HttpPut("Edit/{id}")]
        [CustomAuthorizeAttribute("ConfigurationSetting", "Update")]
        
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateSettingCommand command)
        {
            try
            {
                if (command.Id == id)
                {
                    var result = await _mediator.Send(command);
                    return Ok(result);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }


        [HttpDelete("Delete/{id}")]
        [CustomAuthorizeAttribute("ConfigurationSetting", "Delete")]

        public async Task<BaseResponse> DeleteSetting(Guid id)
        {
            try
            {
                string result = string.Empty;
                return await _mediator.Send(new DeleteSettingCommand { Id = id });

            }
            catch (Exception exp)
            {
                var res = new BaseResponse
                {
                    Success = false,
                    Message = exp.Message
                };
                return res;
            }
        }

        [HttpGet]
        [Route("key")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<SettingDTO>> GetByKey([FromQuery] string Key)
        {
            return await _mediator.Send(new GetSettingByKeyQuery { Key = Key });
        }
    }
}