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

namespace AppDiv.CRVS.API.Controllers
{
    public class CertificateStoreController : ApiControllerBase
    {

        [HttpPost("Tranfer")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> Create(CreateCertificateTransferCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpGet("Recived")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<PaginatedList<CertificateTransferDTO>> Get([FromQuery] GetAllCertificateTransferQuery query)
        {
            // query.UserName = User.Identity.Name;
            return await Mediator.Send(query);
        }
        [HttpGet("Transferred")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
                if (id != null)
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