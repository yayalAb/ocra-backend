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
    public record GetAllCountryForpouch : IRequest<List<AllCountryDto>>
    {

    }

    public class GetAllCountryForpouchHandler : IRequestHandler<GetAllCountryForpouch, List<AllCountryDto>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllCountryForpouchHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AllCountryDto>> Handle(GetAllCountryForpouch request, CancellationToken cancellationToken)
        {
            var query = _AddresslookupRepository.GetAll()
                                .Include(x=>x.AdminTypeLookup)
                                .Where(a => a.AdminLevel == 1 && !a.Status).Select( x=> new AllCountryDto{ 
                                    Id=x.Id,
                                    nameOr =x.AddressName.Value<string>("or"),
                                    nameAm =x.AddressName.Value<string>("am"),
                                    adminLevel =x.AdminLevel,
                                    adminTypeAm =x.AdminTypeLookup!=null? x.AdminTypeLookup.Value.Value<string>("am"):null,
                                    adminTypeOr =x.AdminTypeLookup!=null? x.AdminTypeLookup.Value.Value<string>("or"):null,
                                    mergeStatus= x.Status,
                                }).ToList();
           return query;                     
        }
    }
}
