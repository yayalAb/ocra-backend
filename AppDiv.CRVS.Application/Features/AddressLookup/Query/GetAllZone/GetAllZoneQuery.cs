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
    public record GetAllZoneQuery : IRequest<List<ZoneDTO>>
    {

    }

    public class GetAllZoneQueryHandler : IRequestHandler<GetAllZoneQuery, List<ZoneDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllZoneQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<ZoneDTO>> Handle(GetAllZoneQuery request, CancellationToken cancellationToken)
        {
            var AddressList = await _AddresslookupRepository.GetAllAsync();
            var ZoneList = AddressList.Where(x => x.AdminLevel == 3);
            var FormatedZone = ZoneList.Select(co => new ZoneDTO
            {
                id = co.Id,
                Zone = co.AddressName["en"].ToString(),
                Region = co.ParentAddress?.AddressName["en"].ToString(),
                Country = co.ParentAddress?.ParentAddress?.AddressName["en"].ToString(),
                Code = co.Code,
                StatisticCode = co.StatisticCode

            });

            // var lookups = CustomMapper.Mapper.Map<List<ZoneDTO>>(AddressList);
            return FormatedZone.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }

    }
}