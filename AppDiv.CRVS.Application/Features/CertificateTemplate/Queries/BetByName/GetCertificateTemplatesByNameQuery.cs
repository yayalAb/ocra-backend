
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Domain.Repositories;
using AutoMapper;
using MediatR;
namespace AppDiv.CRVS.Application.Features.CertificateTemplatesLookup.Query.GetCertificateTemplates

{
    public record GetCertificateTemplatesByNameQuery : IRequest<Guid?>
    {
        public string Name { set; get; }
    }

    public class GetCertificateTemplatesQueryByNameHandler : IRequestHandler<GetCertificateTemplatesByNameQuery, Guid?>
    {
        private readonly ICertificateTemplateRepository _CertificateTemplateslookupRepository;

        public GetCertificateTemplatesQueryByNameHandler(ICertificateTemplateRepository CertificateTemplateslookupRepository)
        {
            _CertificateTemplateslookupRepository = CertificateTemplateslookupRepository;
        }
        public Task<Guid?> Handle(GetCertificateTemplatesByNameQuery request, CancellationToken cancellationToken) 
            => Task.FromResult(_CertificateTemplateslookupRepository.GetAll().FirstOrDefault(t => t.CertificateType == request.Name)?.Id);
    }
}