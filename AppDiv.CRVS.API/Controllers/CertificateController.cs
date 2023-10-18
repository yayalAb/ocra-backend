﻿using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Features.Certificates.Command.Create;
using AppDiv.CRVS.Application.Features.Certificates.Command.Delete;
using AppDiv.CRVS.Application.Features.Certificates.Command.Update;
using AppDiv.CRVS.Application.Features.Certificates.Query;
using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Domain;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Newtonsoft.Json.Linq;
using AppDiv.CRVS.Application.Features.Certificates.Command.Verify;
using AppDiv.CRVS.Application.Features.Certificates.Query.Check;
using AppDiv.CRVS.Application.Features.Certificates.Command.ReprintRequest;
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    public class CertificateController : ApiControllerBase
    {

        [HttpGet("Generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,Member,User")]
        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [CustomAuthorizeAttribute("Certificate", "Add")]

        public async Task<object> GetCertificate([FromQuery] Guid id, string? serialNo, bool IsPrint = false, bool checkSerialNumber = true)
        {
            return await Mediator.Send(new GenerateCertificateQuery { Id = id, CertificateSerialNumber = serialNo, IsPrint = IsPrint, CheckSerialNumber = checkSerialNumber });
        }

        [HttpGet("Archive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // ??
        public async Task<object> GetArchive([FromQuery] GenerateArchiveQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost("ArchivePreview")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> GetArchive([FromBody] GenerateArchivePreviewQuery command)
        {
            return await Mediator.Send(command);
        }

        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        [CustomAuthorizeAttribute("Certificate", "Add")]
        
        public async Task<ActionResult> CreateCertificate(CreateCertificateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("Certificate", "ReadAll")]

        public async Task<PaginatedList<CertificateDTO>> Get([FromQuery] GetAllCertificateQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("CheckSerialNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<BaseResponse> Check([FromQuery] CheckSerialNoValidation query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("paidCertificatesByOfficer")]
        [CustomAuthorizeAttribute("Certificate", "ReadAll")]
        public async Task<PaginatedList<AuthenticationRequestListDTO>> Get([FromQuery] GetAllPaidCertificateByCivilRegistrarQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("onWaitingCertificatesByOfficer")]
        [CustomAuthorizeAttribute("Printed", "ReadAll")]

        public async Task<PaginatedList<PaidCertificateDTO>> GetAllOnWaiting([FromQuery] OnWaitingCertificateQuery query)
        {
            return await Mediator.Send(query);
        }


        [HttpGet("unPaidCertificatesByOfficer")]
        [CustomAuthorizeAttribute("Payment", "ReadAll")]
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

        [HttpGet("DraftList")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("InProgress", "ReadAll")]

        public async Task<object> Get([FromQuery] EventDraftListQuery query)
        {
            return await Mediator.Send(query);
        }



        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("InProgress", "ReadAll")]
        public async Task<CertificateDTO> Get(Guid id)
        {
            return await Mediator.Send(new GetCertificateByIdQuery(id));
        }

        [HttpGet("Reprint")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [CustomAuthorizeAttribute("Certificate", "Update")]

        public async Task<object> GetReprint([FromQuery] ReprintCertificateCommand query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("ReprintRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<object> ReprintRequest([FromQuery] ReprintRequestCommand query)
        {
            var result = await Mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }


        }



        [HttpGet("History")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("Certificate", "ReadSingle")]

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
        [CustomAuthorizeAttribute("Certificate", "Update")]

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
        [HttpPut("Verify")]
        // [CustomAuthorizeAttribute("Certificate", "Update")]
        public async Task<ActionResult> VerifyCertificate(VerifyCertificateCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);


        }

        [HttpDelete("Delete/{id}")]
        [CustomAuthorizeAttribute("Certificate", "Delete")]

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

         [HttpGet("AuthenticatedCertificates")]
        public async Task<PaginatedList<PaidCertificateDTO>> GetAllOnWaiting([FromQuery] thisMonthAuthenticatedCertificateList query)
        {
            return await Mediator.Send(query);
        }
    }
}
