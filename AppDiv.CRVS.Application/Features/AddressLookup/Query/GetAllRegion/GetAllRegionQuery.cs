
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllRegion

{
    // Customer query with List<Customer> response
    public record GetAllRegionQuery : IRequest<PaginatedList<RegionDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllRegionQueryHandler : IRequestHandler<GetAllRegionQuery, PaginatedList<RegionDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllRegionQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<PaginatedList<RegionDTO>> Handle(GetAllRegionQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<RegionDTO>
                .CreateAsync(
                     _AddresslookupRepository.GetAll()
                    .Where(a => a.AdminLevel == 2)
                    .Select(c => new RegionDTO
                    {
                        Id = c.Id,
                        Region = c.AddressNameLang,
                        Country = c.ParentAddress != null ? c.ParentAddress.AddressNameLang : null,
                        Code = c.Code,
                        StatisticCode = c.StatisticCode
                    }).ToList()
                    , request.PageCount ?? 1, request.PageSize ?? 10);
        }

    }
}