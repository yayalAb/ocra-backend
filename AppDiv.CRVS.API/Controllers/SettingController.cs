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
    public class SettingController : ControllerBase
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
        public async Task<List<SettingDTO>> Get()
        {
            return await _mediator.Send(new GetAllSettingQuery());
        }

        [HttpPost("Create")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<SettingDTO>> CreateSetting([FromBody] createSettingCommand command, CancellationToken token)
        {
            var result = await _mediator.Send(command, token);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Setting> Get(Guid id)
        {
            return await _mediator.Send(new GetSettingByIdQuery { Id = id });
        }

        [HttpPut("Edit/{id}")]
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
        public async Task<ActionResult> DeleteSetting(Guid id)
        {
            try
            {
                string result = string.Empty;
                result = await _mediator.Send(new DeleteSettingCommand { Id = id });
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
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