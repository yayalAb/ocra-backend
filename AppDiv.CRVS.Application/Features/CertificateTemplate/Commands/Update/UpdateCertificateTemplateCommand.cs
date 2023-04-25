using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Update
{
    public record UpdateCertificateTemplateCommand(UpdateCertificateTemplateRequest CertificateTemplate) : IRequest<object>
    {

    }
}