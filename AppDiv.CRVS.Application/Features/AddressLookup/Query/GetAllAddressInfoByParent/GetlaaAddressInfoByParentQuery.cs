using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddressInfoByParent
{
    // Customer GetlaaAddressInfoByParentQuery with  response
    public class GetlaaAddressInfoByParentQuery : IRequest<List<AddressForMergeDTO>>
    {
        public Guid Id { get; set; }

    }

    public class GetlaaAddressInfoByParentQueryHandler : IRequestHandler<GetlaaAddressInfoByParentQuery, List<AddressForMergeDTO>>
    {
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetlaaAddressInfoByParentQueryHandler(IAddressLookupRepository AddresslookupRepository)
        {
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AddressForMergeDTO>> Handle(GetlaaAddressInfoByParentQuery request, CancellationToken cancellationToken)
        {
            // var Addresss = await _mediator.Send(new GetAllAddressQuery());
            var selectedAddress = _AddresslookupRepository.GetAll().
             Include(ad => ad.AdminTypeLookup)
            .Where(x => x.ParentAddressId == (Guid.Equals(request.Id, Guid.Empty) ? null : request.Id) && !x.Status);
            // var lng = "";
            var formatedAddress = selectedAddress.Select(an => new AddressForMergeDTO
            {
                Id = an.Id,
                ParentAddressId = an.ParentAddressId,
                AddressName = an.AddressName,
                AdminTypeLookupId = an.AdminTypeLookupId,
                AdminLevel = an.AdminLevel,
                StatisticCode = an.StatisticCode,
                Code = an.Code,
                CodePrefix = an.CodePerfix,
                CodePostfix = an.CodePostfix
            });



            return formatedAddress.ToList();            //CustomMapper.Mapper.Map<List<AddressForMergeDTO>>(formatedAddress);
            // return selectedCustomer;
        }
    }
}