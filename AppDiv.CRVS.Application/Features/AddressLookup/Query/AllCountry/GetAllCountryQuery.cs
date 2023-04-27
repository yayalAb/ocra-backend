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
    public record GetAllCountryQuery : IRequest<List<CountryDTO>>
    {

    }

    public class GetAllCountryQueryHandler : IRequestHandler<GetAllCountryQuery, List<CountryDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllCountryQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<CountryDTO>> Handle(GetAllCountryQuery request, CancellationToken cancellationToken)
        {
            var AddressList = await _AddresslookupRepository.GetAllAsync();
            var countryList = AddressList.Where(x => x.ParentAddressId == null);
            var FormatedCounry = countryList.Select(co => new CountryDTO
            {
                id = co.Id,
                Country = co.AddressName["en"].ToString(),
                Code = co.Code,
                StatisticCode = co.StatisticCode
            });

            // var lookups = CustomMapper.Mapper.Map<List<CountryDTO>>(AddressList);
            return FormatedCounry.ToList();

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}