
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
using AppDiv.CRVS.Application.Exceptions;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressById
{
    // Customer GetAddressByIdQuery with  response
    public class GetAddressByIdQuery : IRequest<object>
    {
        public Guid Id { get; private set; }

        public GetAddressByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, object>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAddressByIdQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<object> Handle(GetAddressByIdQuery request, CancellationToken cancellationToken)
        {
            var LookupList = await _AddresslookupRepository.GetAllAsync();
            var lookup = CustomMapper.Mapper.Map<List<AddressDTO>>(LookupList.Where(x => x.Id == request.Id)).FirstOrDefault();
            if (lookup == null)
            {
                throw new NotFoundException($"address with ID {request.Id} is not found");
            }
            return lookup;
            // return selectedCustomer;
        }
    }
}