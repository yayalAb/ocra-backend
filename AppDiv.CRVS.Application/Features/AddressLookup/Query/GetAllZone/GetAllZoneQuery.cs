using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllZone

{
    // Customer query with List<Customer> response
    public record GetAllZoneQuery : IRequest<PaginatedList<ZoneDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }

    public class GetAllZoneQueryHandler : IRequestHandler<GetAllZoneQuery, PaginatedList<ZoneDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllZoneQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<PaginatedList<ZoneDTO>> Handle(GetAllZoneQuery request, CancellationToken cancellationToken)
        {   
              return await PaginatedList<ZoneDTO>
                .CreateAsync(
                     _AddresslookupRepository.GetAll()
                    .Where(a => a.AdminLevel == 3)
                    .Select(a => new ZoneDTO
            {
                Id = a.Id,
                Zone = a.AddressName.Value<string>("en"),
                Region = a.ParentAddress != null? a.ParentAddress.AddressName.Value<string>("en"): null,
                Country = a.ParentAddress !=null  && a.ParentAddress.ParentAddress !=null
                                ? a.ParentAddress.ParentAddress.AddressName.Value<string>("en")
                                :null,
                Code = a.Code,
                StatisticCode = a.StatisticCode

            }).ToList()
                    , request.PageCount ?? 1, request.PageSize ?? 10);

        }

    }
}