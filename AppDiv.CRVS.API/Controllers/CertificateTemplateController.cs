
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create;
using AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update;
using AppDiv.CRVS.Application.Features.CertificateTemplatesLookup.Query.GetAllCertificateTemplates;
using AppDiv.CRVS.Application.Features.CertificateTemplatesLookup.Query.GetCertificateTemplates;
using AppDiv.CRVS.Application.Interfaces;
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
        private readonly IFileService _fileService;

        public CertificateTemplateController(IFileService fileService)
        {
            _fileService = fileService;
        }
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

        [HttpGet("GetByName")]
        // [ProducesDefaultResponseType(typeof(int))]
        public async Task<IActionResult> GetByName([FromQuery] string name)
        {
            var templateId = await Mediator.Send(new GetCertificateTemplatesByNameQuery { Name = name });
            var response = _fileService.getFile(templateId?.ToString(), "CertificateTemplates", null, null);

            return File(response.file,
                            "application/octet-stream"
                            , response.fileName + response.fileExtenion);
        }

    }
}