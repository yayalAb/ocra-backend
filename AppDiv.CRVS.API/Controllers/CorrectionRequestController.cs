using Microsoft.AspNetCore.Mvc;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Commands;
using MediatR;
using Microsoft.AspNetCore.Cors;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Update;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.CorrectionGetById;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.getAllCorrectionRequest;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Delete;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Commands.Approve;
using AppDiv.CRVS.Application.Features.CorrectionRequests.Querys.GetForApproval;
using AppDiv.CRVS.Application.Features.AdoptionEvents.Commands.Update;
using AppDiv.CRVS.Application.Features.BirthEvents.Command.Update;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Features.DeathEvents.Command.Update;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.API.Controllers
{
    [EnableCors("CorsPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class CorrectionRequestController : ApiControllerBase
    {
        private readonly ISender _mediator;
        private readonly ILogger<CorrectionRequestController> _Ilog;
        public CorrectionRequestController(ISender mediator, ILogger<CorrectionRequestController> Ilog)
        {
            _mediator = mediator;
            _Ilog = Ilog; ;
        }
        [HttpPost]
        public async Task<IActionResult> CorrectionREquest([FromBody] CreateCorrectionRequest command)
        {
             var result = await _mediator.Send(command);
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
        public async Task<PaginatedList<CorrectionRequestListDTO>> Get([FromQuery] GetAllCorrectionRequest query)
        {
            return await _mediator.Send(query);
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<AddCorrectionRequest> Get(Guid id)
        {
            return await _mediator.Send(new GetCorrectionRequestQuesry(id));
        }

        [HttpPut("Edit")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] updateCorrectionRequestCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.Success)
                {
                  return Ok(result);
                }

                else
                {
               return BadRequest(result);
                }

            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }


        [HttpPut("Approve")]
        public async Task<ActionResult> Approve([FromBody] ApproveCorrectionRequestCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
        [HttpGet("GetForApproval")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<CorrectionApprovalDTO> GetApproval([FromQuery] GetCorrectionRequestForApproval query)
        {
            return await _mediator.Send(query);
        }

        [HttpDelete("Delete")]
        public async Task<BaseResponse> DeleteLookup([FromBody] DeleteCorrectionRequestCommad command)
        {
            try
            {
                string result = string.Empty;
                return await _mediator.Send(command);
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
    }
}
