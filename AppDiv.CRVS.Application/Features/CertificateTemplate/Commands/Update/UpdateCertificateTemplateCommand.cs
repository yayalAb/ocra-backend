using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update
{
    public record UpdateCertificateTemplateCommand: IRequest<object>
    {
        public Guid Id { get; set; }
        public string CertificateType { get; set; }
        public IFormFile SvgFile { get; set; }
    }
}