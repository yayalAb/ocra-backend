using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetDefualtAddress
{
    // Customer DefualtAddressQuery with  response
    public class DefualtAddressQuery : IRequest<List<AddressForLookupDTO>>
    {
        public bool IsRegion { get; set; } = false;

    }

    public class DefualtAddressQueryHandler : IRequestHandler<DefualtAddressQuery, List<AddressForLookupDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public DefualtAddressQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AddressForLookupDTO>> Handle(DefualtAddressQuery request, CancellationToken cancellationToken)
        {
            // var Addresss = await _mediator.Send(new GetAllAddressQuery());
            var parentID = new Address();
            if (request.IsRegion)
            {
                parentID = _AddresslookupRepository.GetAll()
                          .Where(x => x.IsDefault == true && x.ParentAddressId == null).FirstOrDefault();
            }
            else
            {
                parentID = _AddresslookupRepository.GetAll()
                        .Where(x => x.IsDefault == true && x.ParentAddressId != null).FirstOrDefault();
            }
            if (parentID == null)
            {
                throw new NotFoundException("not found");
            }
            var selectedAddress = _AddresslookupRepository.GetAll().
             Include(ad => ad.AdminTypeLookup)
            .Where(x => x.ParentAddressId == (Guid.Equals(parentID.Id, Guid.Empty) ? null : parentID.Id));
            // var lng = "";
            var formatedAddress = selectedAddress.Select(an => new AddressForLookupDTO
            {
                id = an.Id,
                ParentAddressId = an.ParentAddressId,
                AddressName = an.AddressNameLang,
                AdminType = string.IsNullOrEmpty(an.AdminTypeLookup.ValueLang) ? "" : an.AdminTypeLookup.ValueLang
            });

            return formatedAddress.ToList();            //CustomMapper.Mapper.Map<List<AddressForLookupDTO>>(formatedAddress);
            // return selectedCustomer;
        }
    }
}