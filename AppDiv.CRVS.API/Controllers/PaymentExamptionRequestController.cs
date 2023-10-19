using AppDiv.CRVS.API.Helpers;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Approve;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Create;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Delete;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Command.Update;
using AppDiv.CRVS.Application.Features.PaymentExamptionRequests.Query;
using Microsoft.AspNetCore.Mvc;

namespace AppDiv.CRVS.API.Controllers
{
    public class PaymentExamptionRequestController : ApiControllerBase
    {

        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        [CustomAuthorizeAttribute("Payment", "Add")]
        
        public async Task<ActionResult> CreatePaymentExamptionRequest(CreatePaymentExamptionRequestCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<PaymentExamptionRequestGridDTO>> Get([FromQuery] GetAllPaymentExamptionRequestQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaymentExamptionRequestDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetPaymentExamptionRequestByIdQuery(id));
        }

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdatePaymentExamptionRequestCommand command)
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

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult> DeletePaymentExamptionRequest(Guid id)
        {
            try
            {
                var result = await Mediator.Send(new DeletePaymentExamptionRequestCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpGet("Approve")]
        public async Task<ActionResult> ApprovePaymentExamptionRequest([FromQuery] PaymentExamptionApproval query)
        {
            try
            {
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
        [HttpGet("Detail")]
        public async Task<ActionResult> Detail([FromQuery] GetPaymentExamptionDetailById query)
        {
            try
            {
                var result = await Mediator.Send(query);
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

    }
}

