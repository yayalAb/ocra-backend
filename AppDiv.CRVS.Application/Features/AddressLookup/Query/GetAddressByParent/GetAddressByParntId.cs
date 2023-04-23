using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Mapper;
using MediatR;


namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressByParent
{
    // Customer GetAddressByParntId with  response
    public class GetAddressByParntId : IRequest<List<AddressDTO>>
    {
        public Guid Id { get; set; }

    }

    public class GetAddressByParntIdHandler : IRequestHandler<GetAddressByParntId, List<AddressDTO>>
    {
        private readonly IMediator _mediator;

        public GetAddressByParntIdHandler(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<List<AddressDTO>> Handle(GetAddressByParntId request, CancellationToken cancellationToken)
        {
            var Addresss = await _mediator.Send(new GetAllAddressQuery());
            var selectedAddress = Addresss.Where(x => x.ParentAddressId == request.Id);
            return CustomMapper.Mapper.Map<List<AddressDTO>>(selectedAddress);
            // return selectedCustomer;
        }
    }
}