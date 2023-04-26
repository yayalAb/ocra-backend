using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAllAddress;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;


namespace AppDiv.CRVS.Application.Features.AddressLookup.Query.GetAddressbyAdminstrativeLevel
{
    // Customer GetByAdminstrativeLevelQuery with  response
    public class GetByAdminstrativeLevelQuery : IRequest<List<AddressDTO>>
    {
        public Guid Id { get; set; }

    }

    public class GetByAdminstrativeLevelQueryHandler : IRequestHandler<GetByAdminstrativeLevelQuery, List<AddressDTO>>
    {
        private readonly IMediator _mediator;
        private readonly IAddressLookupRepository _AddresslookupRepository;

        public GetByAdminstrativeLevelQueryHandler(IMediator mediator, IAddressLookupRepository AddresslookupRepository)
        {
            _mediator = mediator;
            _AddresslookupRepository = AddresslookupRepository;
        }
        public async Task<List<AddressDTO>> Handle(GetByAdminstrativeLevelQuery request, CancellationToken cancellationToken)
        {

            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "ChildAddresses", NavigationPropertyType.COLLECTION }

                                                };
            // var userData = await _userRepository.GetWithAsync(explicitLoadedProperties);
            var Addresss = await _AddresslookupRepository.GetAllWithAsync("explicitLoadedProperties");
            // var selectedAddress = Addresss.Where(x => x.AdminLevelLookupId == request.Id);
            return CustomMapper.Mapper.Map<List<AddressDTO>>(Addresss);
            // return selectedCustomer;
        }
    }
}