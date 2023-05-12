
using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Payments.Command.Create;
// using AppDiv.CRVS.Application.Features.Payments.Command.Update;
// using AppDiv.CRVS.Application.Features.Payments.Query;

namespace AppDiv.CRVS.API.Controllers
{
    public class PaymentController : ApiControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> createPayment([FromBody] CreatePaymentCommand command)
        {

            return Ok(await Mediator.Send(command));
        }

        // [HttpPut]
        // public async Task<IActionResult> UpdatePayment([FromBody] UpdatePaymentCommand command)
        // {

        //     return Ok(await Mediator.Send(command));
        // }

        // [HttpGet]
        // public async Task<IActionResult> GetAllPayments([FromQuery] GetAllPaymentsQuery query)
        // {

        //     return Ok(await Mediator.Send(query));
        // }
        // [HttpGet("{id}")]
        // public async Task<IActionResult> GetPaymentById(Guid id)
        // {

        //     return Ok(await Mediator.Send(new GetPaymentByIdQuery{Id = id}));
        // }
    }
}