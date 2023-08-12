
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Domain.Repositories;
using AutoMapper;
using MediatR;
using AppDiv.CRVS.Utility.Services;

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
            var CertificateTemplateList= _CertificateTemplateslookupRepository.GetAllAsync();

            return await  PaginatedList<FetchCertificateTemplateDTO>
                            .CreateAsync(
                                CertificateTemplateList.Select(x => new FetchCertificateTemplateDTO
                                {
                                    Id =x.Id,
                                    CertificateType =x.CertificateType,
                                    CreatedAt =new CustomDateConverter(x.CreatedAt).ethiopianDate,
                                    ModifiedAt =new CustomDateConverter(x.ModifiedAt).ethiopianDate,
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);

        }
    }
}