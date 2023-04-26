
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.CertificateTemplatesLookup.Query.GetAllCertificateTemplates

{
    // Customer query with List<Customer> response
    public record GetAllCertificateTemplatesQuery : IRequest<PaginatedList<FetchCertificateTemplateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllCertificateTemplatesQueryHandler : IRequestHandler<GetAllCertificateTemplatesQuery, PaginatedList<FetchCertificateTemplateDTO>>
    {
        private readonly ICertificateTemplateRepository _CertificateTemplateslookupRepository;
        private readonly IMapper _mapper;

        public GetAllCertificateTemplatesQueryHandler(ICertificateTemplateRepository CertificateTemplateslookupRepository, IMapper mapper )
        {
            _CertificateTemplateslookupRepository = CertificateTemplateslookupRepository;
            _mapper = mapper;
        }
        public async Task<PaginatedList<FetchCertificateTemplateDTO>> Handle(GetAllCertificateTemplatesQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<FetchCertificateTemplateDTO>
                        .CreateAsync(
                           await _CertificateTemplateslookupRepository.GetAllAsync().ProjectToListAsync<FetchCertificateTemplateDTO>(_mapper.ConfigurationProvider), request.PageCount ?? 1, request.PageSize ?? 10);

        }
    }
}