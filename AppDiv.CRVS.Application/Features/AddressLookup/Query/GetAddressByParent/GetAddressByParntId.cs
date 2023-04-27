using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Mapper;
using MediatR;
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
        private readonly IMediator _mediator;

        public GetAddressByParntIdHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<List<AddressForLookupDTO>> Handle(GetAddressByParntId request, CancellationToken cancellationToken)
        {
            var Addresss = await _mediator.Send(new GetAllAddressQuery());
            var selectedAddress = Addresss.Where(x => x.ParentAddressId == request.Id);
            // var lng = "";
            var formatedAddress = selectedAddress.Select(an => new AddressForLookupDTO
            {
                id = an.id,
                ParentAddressId = an.ParentAddressId,
                AddressName = an.AddressName.Value<string>("en")
            });

            return formatedAddress.ToList();            //CustomMapper.Mapper.Map<List<AddressForLookupDTO>>(formatedAddress);
            // return selectedCustomer;
        }
    }
}