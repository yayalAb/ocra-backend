using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Features.Certificates.Command.Delete;
using AppDiv.CRVS.Application.Features.Certificates.Command.Update;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.Domain;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.API.Controllers
{
    public class CertificateController : ApiControllerBase
    {

        [HttpGet("Generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetCertificate([FromQuery] Guid id, string? serialNo, bool IsPrint = false)
        {
            return await Mediator.Send(new GenerateCertificateQuery { Id = id, CertificateSerialNumber = serialNo, IsPrint = IsPrint });
        }

        [HttpPost("CorrectionRequest")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> CreateCertificate(CreateCertificateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<CertificateDTO>> Get([FromQuery] GetAllCertificateQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("paidCertificatesByOfficer")]
        public async Task<PaginatedList<PaidCertificateDTO>> Get([FromQuery] GetAllPaidCertificateByCivilRegistrarQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("unPaidCertificatesByOfficer")]
        public async Task<PaginatedList<UnPaidCertificateDTO>> Get([FromQuery] GetAllUnPaidCertificateByCivilRegistrarQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("lastGeneratedIdInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> Get([FromQuery] GetLastGeneratedEventIdByOfficerQuery query)
        {
            return await Mediator.Send(query);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<CertificateDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetCertificateByIdQuery(id));
        }

        [HttpGet("Reprint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<CertificateDTO> GetReprint([FromQuery] ReprintCertificateCommand query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("History")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<EventHistoryDto> GetHistory([FromQuery] GetCertificateHistoryQuery query)
        {
            return await Mediator.Send(query);
        }




        [HttpGet("Event/{eventId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<CertificateDTO>> GetByEventId(Guid eventId)
        {
            return await Mediator.Send(new GetCertificateByEventIdQuery(eventId));
        }

        [HttpPut("Edit/{id}")]
        public async Task<ActionResult> Edit(Guid id, [FromBody] UpdateCertificateCommand command)
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
        public async Task<ActionResult> DeleteCertificate(Guid id)
        {
            try
            {
                string result = string.Empty;
                result = await Mediator.Send(new DeleteCertificateCommand(id));
                return Ok(result);
            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }
    }
}