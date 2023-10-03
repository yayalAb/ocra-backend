using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Features.User.Command.Create;
using AppDiv.CRVS.Application.Features.User.Command.Delete;
using AppDiv.CRVS.Application.Features.User.Command.Update;
using AppDiv.CRVS.Application.Features.User.Query;
using AppDiv.CRVS.Application.Features.User.Query.GetUserById;
using AppDiv.CRVS.Domain;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using AppDiv.CRVS.Application.Features.User.Command.CheckDuplication;
using AppDiv.CRVS.Application.Features.Auth.YourTeam;
using AppDiv.CRVS.Application.Features.User.Command.UpdateUserName;

namespace AppDiv.CRVS.API.Controllers
{
    public class UserController : ApiControllerBase
    {

        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> CreateUser(CreateUserCommand command)
        {
            var res = await Mediator.Send(command);
            if (!res.Success)
            {
                return BadRequest(res);
            }
            return Ok(res);
        }


        [HttpGet("GetUserDetailsByUserName/{userName}")]
        [ProducesDefaultResponseType(typeof(UserResponseDTO))]
        public async Task<IActionResult> GetUserDetailsByUserName(string userName)
        {
            var result = await Mediator.Send(new GetUserDetailsByUserNameQuery() { UserName = userName });
            return Ok(result);
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<UserResponseDTO>> Get([FromQuery] GetAllUserQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<FetchSingleUserResponseDTO> Get(string id)
        {
            return await Mediator.Send(new GetUserByIdQuery(id));
        }


        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(string id, [FromBody] UpdateUserCommand command)
        {
            try
            {
                if (command.Id == id)
                {
                    var result = await Mediator.Send(command);
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

        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteUser([FromBody] DeleteUserCommand command)
        {
            try
            {
                var response = await Mediator.Send(command);
                if (response.Status == 200)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPut("ChangeStatus")]
        public async Task<ActionResult> Activate([FromBody] ActivateUserCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                if (result.Status != 200)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPut("UpdateUserNameOrEmail")]
        public async Task<ActionResult> UpdateUserName([FromBody] UpdateUserNameCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
        [HttpPost("CheckDuplication")]
        public async Task<ActionResult> CheckDuplicateEmail([FromBody] CheckDuplicationCommand command)
        {
            try
            {
                return Ok(await Mediator.Send(command));
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
        [HttpGet("YourTeam")]
        public async Task<ActionResult> GetYourTeam([FromBody] GetYourTeamQuery query)
        {
            try
            {
                return Ok(await Mediator.Send(query));
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

    }
}