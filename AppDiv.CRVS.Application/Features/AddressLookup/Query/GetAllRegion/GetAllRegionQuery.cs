
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllRegion

{
    // Customer query with List<Customer> response
    public record GetAllRegionQuery : IRequest<PaginatedList<RegionDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
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
            var query = _AddresslookupRepository.GetAll()
                    .Where(a => a.AdminLevel == 2);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                query = query.Where(a => EF.Functions.Like(a.AddressNameStr, "%" + request.SearchString + "%")
                             || (a.ParentAddress != null && EF.Functions.Like(a.ParentAddress.AddressNameStr, "%" + request.SearchString + "%"))
                             );
            }
            return await PaginatedList<RegionDTO>
                .CreateAsync(
                   query
                    .Select(c => new RegionDTO
                    {
                        Id = c.Id,
                        Region = c.AddressNameLang,
                        Country = c.ParentAddress != null ? c.ParentAddress.AddressNameLang : null,
                        Code = c.Code,
                        StatisticCode = c.StatisticCode,
                        AdminType = string.IsNullOrEmpty(c.AdminTypeLookup.ValueLang) ? null : c.AdminTypeLookup.ValueLang
                    })
                    , request.PageCount ?? 1, request.PageSize ?? 10);
        }

    }
}