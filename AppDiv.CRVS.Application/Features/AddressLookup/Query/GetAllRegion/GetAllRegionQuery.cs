
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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllRegion

{
    // Customer query with List<Customer> response
    public record GetAllRegionQuery : IRequest<List<RegionDTO>>
    {

    }

    public class GetAllRegionQueryHandler : IRequestHandler<GetAllRegionQuery, List<RegionDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllRegionQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<RegionDTO>> Handle(GetAllRegionQuery request, CancellationToken cancellationToken)
        {
            var AddressList = await _AddresslookupRepository.GetAllAsync();
            var countryList = AddressList.Where(x => x.AdminLevel == 2);
            var FormatedRegion = countryList.Select(co => new RegionDTO
            {
                id = co.Id,
                Region = co.AddressName["en"].ToString(),
                Country = co.ParentAddress?.AddressName["en"].ToString(),
                Code = co.Code,
                StatisticCode = co.StatisticCode
            });

            // var lookups = CustomMapper.Mapper.Map<List<RegionDTO>>(AddressList);
            return FormatedRegion.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }

    }
}