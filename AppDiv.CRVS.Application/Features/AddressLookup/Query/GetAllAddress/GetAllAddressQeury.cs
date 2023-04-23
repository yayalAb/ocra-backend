
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

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress

{
    // Customer query with List<Customer> response
    public record GetAllAddressQuery : IRequest<List<AddressDTO>>
    {

    }

    public class GetAllAddressQueryHandler : IRequestHandler<GetAllAddressQuery, List<AddressDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAllAddressQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AddressDTO>> Handle(GetAllAddressQuery request, CancellationToken cancellationToken)
        {
            var LookupList = await _AddresslookupRepository.GetAllAsync();
            var lookups = CustomMapper.Mapper.Map<List<AddressDTO>>(LookupList);
            return lookups;

            // return (List<Customer>)await _customerQueryRepository.GetAllAsync();
        }
    }
}