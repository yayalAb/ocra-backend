
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update;
using AppDiv.CRVS.Application.Features.CertificateTemplatesLookup.Query.GetAllCertificateTemplates;
using AppDiv.CRVS.Domain;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace AppDiv.CRVS.API.Controllers
{
    public class CertificateTemplateController : ApiControllerBase
    {

        [HttpPost("Create")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> CreateCertificateTemplate([FromForm] CreateCertificateTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpPost("Update")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> UpdateCertificateTemplate([FromForm] UpdateCertificateTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<ActionResult> Get([FromQuery] GetAllCertificateTemplatesQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

    }
}