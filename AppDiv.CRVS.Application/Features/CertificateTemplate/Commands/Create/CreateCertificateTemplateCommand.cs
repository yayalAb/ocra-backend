using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public record CreateCertificateTemplateCommand : IRequest<object>
    {
        public string CertificateType { get; set; }
        public IFormFile SvgFile { get; set; }
    }
}