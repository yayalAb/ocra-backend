using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressByParent
{
    // Customer GetAddressByParntId with  response
    public class GetAddressByParntId : IRequest<List<AddressForLookupDTO>>
    {
        public Guid Id { get; set; }

    }

    public class GetAddressByParntIdHandler : IRequestHandler<GetAddressByParntId, List<AddressForLookupDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetAddressByParntIdHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AddressForLookupDTO>> Handle(GetAddressByParntId request, CancellationToken cancellationToken)
        {
            // var Addresss = await _mediator.Send(new GetAllAddressQuery());
            var selectedAddress = _AddresslookupRepository.GetAll().
             Include(ad => ad.AdminTypeLookup)
            .Where(x => x.ParentAddressId == (Guid.Equals(request.Id, Guid.Empty) ? null : request.Id));
            // var lng = "";
            var formatedAddress = selectedAddress.Select(an => new AddressForLookupDTO
            {
                id = an.Id,
                ParentAddressId = an.ParentAddressId,
                AddressName = an.AddressNameLang,
                AdminType = string.IsNullOrEmpty(an.AdminTypeLookup.ValueLang) ? "" : an.AdminTypeLookup.ValueLang,
                AdminLevel = an.AdminLevel
            });

            return formatedAddress.ToList();            //CustomMapper.Mapper.Map<List<AddressForLookupDTO>>(formatedAddress);
            // return selectedCustomer;
        }
    }
}