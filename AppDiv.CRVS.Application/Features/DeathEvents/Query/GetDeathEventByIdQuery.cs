using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using MediatR;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByTransactionId;

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
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMediator _mediator;

        public GetDeathEventByIdHandler(IDeathEventRepository deathEventRepository, 
            IDateAndAddressService AddressService, 
            IEventDocumentService eventDocumentService,
            IMediator mediator)
        {
            _deathEventRepository = deathEventRepository;
            _AddressService = AddressService;
            _eventDocumentService = eventDocumentService;
            this._mediator = mediator;
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
            DeathEvent!.Event.EventAddressResponseDTO = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventAddressId)!;
            DeathEvent!.Event.EventOwener.BirthAddressResponseDTO = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventOwener?.BirthAddressId)!;
            DeathEvent!.Event.EventOwener.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventOwener?.ResidentAddressId)!;
            if (DeathEvent?.Event?.EventRegistrar?.RegistrarInfo != null)
            {
                DeathEvent!.Event.EventRegistrar.RegistrarInfo.BirthAddressResponseDTO = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventRegistrar.RegistrarInfo?.BirthAddressId)!;
                DeathEvent!.Event.EventRegistrar.RegistrarInfo.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(DeathEvent?.Event?.EventRegistrar.RegistrarInfo?.ResidentAddressId)!;
            }
            DeathEvent.Event.fingerPrints = new
            {
                Deceased = _eventDocumentService.getSingleFingerprintUrls(DeathEvent.Event.EventOwener?.Id.ToString()),
                Registrar = _eventDocumentService.getSingleFingerprintUrls(DeathEvent.Event.EventRegistrar?.RegistrarInfo?.Id.ToString())
            };
            return DeathEvent!;
        }
    }
}