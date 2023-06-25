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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.AllCountry

{
    // Customer query with List<Customer> response
    public record GetAllCountryQuery : IRequest<PaginatedList<CountryDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, PaginatedList<CountryDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllCountryQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<PaginatedList<CountryDTO>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            var query = _AddresslookupRepository.GetAll()
                                .Where(a => a.AdminLevel == 1);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                query = query.Where(a => EF.Functions.Like(a.AddressNameStr, "%" + request.SearchString + "%"));
            }
            return await PaginatedList<CountryDTO>
                            .CreateAsync(
                               query
                                .Select(c => new CountryDTO
                                {
                                    Id = c.Id,
                                    Country = c.AddressNameLang,
                                    Code = c.Code,
                                    StatisticCode = c.StatisticCode
                                })
                                , request.PageCount ?? 1, request.PageSize ?? 10);

        }
    }
}