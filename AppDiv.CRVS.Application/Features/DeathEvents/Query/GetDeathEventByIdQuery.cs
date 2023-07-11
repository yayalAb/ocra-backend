using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;
using AppDiv.CRVS.Application.Interfaces;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetDeathEventByIdQuery : IRequest<DeathEventDTO>
    {
        public Guid Id { get; private set; }

        public GetDeathEventByIdQuery(Guid Id)
        {
            this.Id = Id;
        }

    }

    public class GetDeathEventByIdHandler : IRequestHandler<GetDeathEventByIdQuery, DeathEventDTO>
    {
        private readonly IDeathEventRepository _deathEventRepository;
        private readonly IDateAndAddressService _AddressService;


        public GetDeathEventByIdHandler(IDeathEventRepository deathEventRepository, IDateAndAddressService AddressService)
        {
            _deathEventRepository = deathEventRepository;
            _AddressService = AddressService;
        }
        public async Task<DeathEventDTO> Handle(GetDeathEventByIdQuery request, CancellationToken cancellationToken)
        {
            // Get the death event by id.
            var selectedDeathEvent = await _deathEventRepository.GetIncludedAsync(request.Id);
            // Map to dto
            var DeathEvent = CustomMapper.Mapper.Map<DeathEventDTO>(selectedDeathEvent);
            // Set the supporting document to null if it is empty list.
            DeathEvent.Event.EventSupportingDocuments = (DeathEvent?.Event?.EventSupportingDocuments?.Count == 0 ? null : DeathEvent?.Event?.EventSupportingDocuments)!;
            if (DeathEvent?.Event.PaymentExamption != null)
                DeathEvent.Event.PaymentExamption.SupportingDocuments = (DeathEvent?.Event?.PaymentExamption?.SupportingDocuments?.Count == 0 ? null : DeathEvent?.Event?.PaymentExamption?.SupportingDocuments)!;
            // Get the formated addresses
            DeathEvent!.Event.EventAddress = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventAddressId)!;
            DeathEvent!.Event.EventOwener.BirthAddress = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventOwener?.BirthAddressId)!;
            DeathEvent!.Event.EventOwener.ResidentAddress = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventOwener?.ResidentAddressId)!;
            if (DeathEvent?.Event.EventRegistrar.RegistrarInfo != null)
            {
                DeathEvent!.Event.EventRegistrar.RegistrarInfo.BirthAddress = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventRegistrar.RegistrarInfo?.BirthAddressId)!;
                DeathEvent!.Event.EventRegistrar.RegistrarInfo.ResidentAddress = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventRegistrar.RegistrarInfo?.ResidentAddressId)!;
            }
            return DeathEvent!;
        }
    }
}