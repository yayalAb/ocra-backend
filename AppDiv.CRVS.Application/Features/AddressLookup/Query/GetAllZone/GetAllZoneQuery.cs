using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        public string? SearchString { get; set; }
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
            var query = _AddresslookupRepository.GetAll()
                    .Where(a => a.AdminLevel == 3);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                query = query.Where(a =>
                        EF.Functions.Like(a.AddressNameStr, "%" + request.SearchString + "%")
                        || (a.ParentAddress != null && EF.Functions.Like(a.ParentAddress.AddressNameStr, "%" + request.SearchString + "%"))
                        || ((a.ParentAddress != null && a.ParentAddress.ParentAddress != null) && EF.Functions.Like(a.ParentAddress.ParentAddress.AddressNameStr, "%" + request.SearchString + "%"))
                        );
            }
            return await PaginatedList<ZoneDTO>
              .CreateAsync(
                  query
                  .Select(a => new ZoneDTO
                  {
                      Id = a.Id,
                      Zone = a.AddressNameLang,
                      Region = a.ParentAddress != null ? a.ParentAddress.AddressNameLang : null,
                      Country = a.ParentAddress != null && a.ParentAddress.ParentAddress != null
                              ? a.ParentAddress.ParentAddress.AddressNameLang
                              : null,
                      Code = a.Code,
                      StatisticCode = a.StatisticCode,
                      AdminType = string.IsNullOrEmpty(a.AdminTypeLookup.ValueLang) ? null : a.AdminTypeLookup.ValueLang

                  })
                  , request.PageCount ?? 1, request.PageSize ?? 10);

        }

    }
}