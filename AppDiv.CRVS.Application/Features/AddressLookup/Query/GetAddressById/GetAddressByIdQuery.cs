
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressById
{
    // Customer GetAddressByIdQuery with  response
    public class GetAddressByIdQuery : IRequest<List<AddressDTO>>
    {
        public Guid Id { get; private set; }

        public GetAddressByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, List<AddressDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAddressByIdQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AddressDTO>> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var LookupList = await _AddresslookupRepository.GetAllAsync();
            var lookups = CustomMapper.Mapper.Map<List<AddressDTO>>(LookupList.Where(x => x.Id == request.Id));
            return lookups;
            // return selectedCustomer;
        }
    }
}