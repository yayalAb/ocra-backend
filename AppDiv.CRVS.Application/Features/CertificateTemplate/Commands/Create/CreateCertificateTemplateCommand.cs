using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Commands.Create
{
    public record CreateCertificateTemplateCommand(CreateCertificateTemplateRequest CertificateTemplate) : IRequest<object>
    {

    }
}