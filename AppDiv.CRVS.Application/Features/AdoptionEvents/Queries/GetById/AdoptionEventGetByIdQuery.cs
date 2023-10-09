
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByTransactionId;
using AppDiv.CRVS.Application.Service;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.AdoptionEvents.Queries.GetById
{
    // Customer query with List<Customer> response
    public record AdoptionEventGetByIdQuery : IRequest<AdoptionDTO>
    {
        public Guid Id { get; set; }
    }

    public class AdoptionEventGetByIdQueryHandler : IRequestHandler<AdoptionEventGetByIdQuery, AdoptionDTO>
    {
        private readonly IAdoptionEventRepository _adoptionEventRepository;
        private readonly IDateAndAddressService _AddressService;
        private readonly IMapper _mapper;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMediator _mediator;

        public AdoptionEventGetByIdQueryHandler(IDateAndAddressService AddressService, 
            IAdoptionEventRepository adoptionEventRepository, 
            IMapper mapper, 
            IEventDocumentService EventDocumentService,
            IMediator mediator)
        {
            _adoptionEventRepository = adoptionEventRepository;
            _AddressService = AddressService;
            _mapper = mapper;
            _eventDocumentService = EventDocumentService;
            this._mediator = mediator;
        }
        public async Task<AdoptionDTO> Handle(AdoptionEventGetByIdQuery request, CancellationToken cancellationToken)
        {

            var adoptionEvent = await _adoptionEventRepository
                            .GetAll().Where(m => m.Id == request.Id)
                            .Include(m => m.AdoptiveFather)
                            .ThenInclude(b => b.ContactInfo)
                            .Include(m => m.AdoptiveMother)
                            .Include(m => m.Event)
                            .Include(m => m.Event.EventOwener)
                            .ThenInclude(e => e.ContactInfo)
                            .Include(m => m.Event.EventSupportingDocuments)
                            .Include(m => m.Event.PaymentExamption)
                            .ThenInclude(p => p.SupportingDocuments)
                            .ProjectTo<AdoptionDTO>(_mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(cancellationToken);
            if (adoptionEvent == null)
            {
                throw new NotFoundException($"Adoption Event with id {request.Id} not found");
            }
            adoptionEvent.BeforeAdoptionAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.BeforeAdoptionAddressId);
            adoptionEvent.AdoptiveFather.BirthAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.AdoptiveFather?.BirthAddressId);
            adoptionEvent.AdoptiveFather.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.AdoptiveFather?.ResidentAddressId);
            adoptionEvent.AdoptiveMother.BirthAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.AdoptiveMother?.BirthAddressId);
            adoptionEvent.AdoptiveMother.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.AdoptiveMother?.ResidentAddressId);
            adoptionEvent.Event.EventOwener.BirthAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.Event?.EventOwener?.BirthAddressId);
            adoptionEvent.Event.EventOwener.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(adoptionEvent?.Event?.EventOwener?.ResidentAddressId);
            adoptionEvent.CourtCase.Court.CourtAddress = await _AddressService.FormatedAddress(adoptionEvent?.CourtCase?.Court?.AddressId);

            adoptionEvent.Event.fingerPrints = new
            {
                Mother = _eventDocumentService.getSingleFingerprintUrls(adoptionEvent.AdoptiveMother?.Id.ToString()),
                Father = _eventDocumentService.getSingleFingerprintUrls(adoptionEvent.AdoptiveFather?.Id.ToString()),
                Child = _eventDocumentService.getSingleFingerprintUrls(adoptionEvent.Event.EventOwener?.Id.ToString())
            };

            return adoptionEvent;
        }
    }
}