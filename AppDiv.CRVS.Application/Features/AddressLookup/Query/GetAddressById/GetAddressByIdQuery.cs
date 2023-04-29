
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
            // var selectedAddress = _AddresslookupRepository.GetAll().Where(x => x.Id == request.Id);



            var selectedAddress = _AddresslookupRepository.GetAll().Where(x => x.Id == request.Id).Select(ad => new AddressDTO
            {
                id = ad.Id,
                AddressName = ad.AddressName,
                StatisticCode = ad.StatisticCode,
                Code = ad.Code,
                AdminLevel = ad.AdminLevel,
                AreaTypeLookupId = ad.AreaTypeLookupId,
                ParentAddressId = ad.AreaTypeLookupId,
                ParentAddress = CustomMapper.Mapper.Map<AddressDTO>(ad.ParentAddress)
            });
            return CustomMapper.Mapper.Map<object>(selectedAddress);
            // return selectedCustomer;
        }
    }
}