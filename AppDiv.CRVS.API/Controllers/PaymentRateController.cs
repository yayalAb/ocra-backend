using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Features.PaymentRates.Command.Create;
using AppDiv.CRVS.Application.Features.PaymentRates.Command.Delete;
using AppDiv.CRVS.Application.Features.PaymentRates.Command.Update;
using AppDiv.CRVS.Application.Features.PaymentRates.Query;
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

namespace AppDiv.CRVS.API.Controllers
{
    public class PaymentRateController : ApiControllerBase
    {

        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        
        [CustomAuthorizeAttribute("PaymentRateSetting", "Add")]
        public async Task<ActionResult> CreatePaymentRate(CreatePaymentRateCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }

        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("PaymentRateSetting", "ReadAll")]

        public async Task<PaginatedList<FetchPaymentRateDTO>> Get([FromQuery] GetAllPaymentRateQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("PaymentRateSetting", "Update")]

        public async Task<PaymentRateDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetPaymentRateByIdQuery(id));
        }


        [HttpPut("Edit/{id}")]
        [CustomAuthorizeAttribute("PaymentRateSetting", "Update")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdatePaymentRateCommand command)
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
        [CustomAuthorizeAttribute("PaymentRateSetting", "Delete")]

        public async Task<ActionResult> DeletePaymentRate([FromBody] DeletePaymentRateCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                if (result.Success)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

    }
}