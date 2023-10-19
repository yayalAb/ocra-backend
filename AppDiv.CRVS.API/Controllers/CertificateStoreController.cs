using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Customers.Query;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllUser;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Create;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Command.Update;
using AppDiv.CRVS.Application.Features.CertificateStores.CertificateTransfers.Query;
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
using AppDiv.CRVS.API.Helpers;

namespace AppDiv.CRVS.API.Controllers
{
    public class CertificateStoreController : ApiControllerBase
    {

        [HttpPost("Tranfer")]
        // [ProducesDefaultResponseType(typeof(int))]
        [CustomAuthorizeAttribute("CertificateStoreTransfer", "Add")]

        public async Task<ActionResult> Create(CreateCertificateTransferCommand command)
        {
            var result = await Mediator.Send(command);
            if(!result.Success)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("Recived")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("CertificateStoreReceive", "ReadAll")]

        public async Task<PaginatedList<CertificateTransferDTO>> Get([FromQuery] GetAllCertificateTransferQuery query)
        {
            // query.UserName = User.Identity.Name;
            return await Mediator.Send(query);
        }
        [HttpGet("Transferred")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("CertificateStoreTransfer", "ReadAll")]

        public async Task<PaginatedList<CertificateTransferDTO>> GetTransfers([FromQuery] GetCertificateTransferByUserQuery query)
        {
            // query.UserName = User.Identity.Name;
            return await Mediator.Send(query);
        }

        // [HttpGet("{id}")]
        // [ProducesResponseType(StatusCodes.Status200OK)]
        // public async Task<CertificateTransferDTO> Get(Guid id)
        // {
        //     return await Mediator.Send(new GetCertificateTransferByUserQuery);
        // }


        [HttpPut("Accept/{id}")]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                if (id != Guid.Empty)
                {
                    var result = await Mediator.Send(new UpdateCertificateTransferCommand { Id = id });
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

        [HttpPost("Damaged")]
        // [ProducesDefaultResponseType(typeof(int))]
        [CustomAuthorizeAttribute("CertificateStoreDamaged", "Add")]


        public async Task<ActionResult> Create(CreateDamagedCertificatesCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("Damaged/{id}")]
        [CustomAuthorizeAttribute("CertificateStoreDamaged", "Update")]

        public async Task<ActionResult> Update([FromRoute] Guid id, [FromBody] UpdateDamagedCertificatesCommand command)
        {
            try
            {
                if (id != Guid.Empty)
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

        [HttpGet("Damaged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("CertificateStoreDamaged", "ReadAll")]

        public async Task<PaginatedList<DamagedCertificatesDTO>> GetDamaged([FromQuery] GetAllDamagedCertificatesQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("Damaged/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [CustomAuthorizeAttribute("CertificateStoreDamaged", "Update")]

        public async Task<DamagedCertificatesDTO> GetDamagedById(Guid id)
        {
            return await Mediator.Send(new GetDamagedCertificateByIdQuery { Id = id });
        }

        // [HttpDelete("Delete/{id}")]
        // public async Task<ActionResult> Delete(Guid id)
        // {
        //     try
        //     {
        //         var result = await Mediator.Send(new DeleteCerificateTransferCommand(id));
        //         if (result.Success)
        //             return Ok(result);
        //         else
        //             return BadRequest(result);
        //     }
        //     catch (Exception exp)
        //     {
        //         return BadRequest(exp.Message);
        //     }
        // }

    }
}