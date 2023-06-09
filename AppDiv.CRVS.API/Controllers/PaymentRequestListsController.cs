using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.Courts.Commmands.Create;
using AppDiv.CRVS.Application.Features.Courts.Query.GetAllCourt;
using AppDiv.CRVS.Application.Features.Courts.Commmands.Delete;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.Courts.Commmands.Update;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetLookupById;
using AppDiv.CRVS.Application.Features.Courts.Query.GetById;
using AppDiv.CRVS.Application.Features.PaymentRequest.PaymentRequestQuery;

namespace AppDiv.CRVS.API.Controllers
{
    public class PaymentRequestListsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<PaymentRequestListDTO>>> GetAll()
        {

            return Ok(await Mediator.Send(new PaymentRequestListQuery()));
        }
    }
}