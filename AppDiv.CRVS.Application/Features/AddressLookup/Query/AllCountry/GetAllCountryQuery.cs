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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.AllCountry

{
    // Customer query with List<Customer> response
    public record GetAllCountryQuery : IRequest<PaginatedList<CountryDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
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
            return await PaginatedList<CountryDTO>
                            .CreateAsync(
                                 _AddresslookupRepository.GetAll()
                                .Where(a => a.ParentAddress == null)
                                .Select(c => new CountryDTO
                                        {
                                            Id = c.Id,
                                            Country = c.AddressName.Value<string>("en"),
                                            Code = c.Code,
                                            StatisticCode = c.StatisticCode
                                        }).ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);

        }
    }
} 