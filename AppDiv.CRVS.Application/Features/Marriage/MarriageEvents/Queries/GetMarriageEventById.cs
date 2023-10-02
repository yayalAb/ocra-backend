
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.MarriageEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Notification.Queries.GetNotificationByTransactionId;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.MarriageEvents.Query
{
    // Customer query with List<Customer> response
    public record GetMarriageEventByIdQuery : IRequest<UpdateMarriageEventCommand>
    {
        public Guid Id { get; set; }
        public Guid? TransactionId { get; set; }
    }

    public class GetMarriageEventByIdQueryHandler : IRequestHandler<GetMarriageEventByIdQuery, UpdateMarriageEventCommand>
    {
        private readonly IMarriageEventRepository _MarriageEventRepository;
        private readonly IDateAndAddressService _AddressService;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GetMarriageEventByIdQueryHandler(IMarriageEventRepository MarriageEventRepository, 
            IMapper mapper, 
            IDateAndAddressService AddressService, 
            IEventDocumentService eventDocumentService,
            IMediator mediator)
        {
            _MarriageEventRepository = MarriageEventRepository;
            _AddressService = AddressService;
            _eventDocumentService = eventDocumentService;
            this._mediator = mediator;
            _mapper = mapper;
        }
        public async Task<UpdateMarriageEventCommand> Handle(GetMarriageEventByIdQuery request, CancellationToken cancellationToken)
        {

            var MarriageEvent = _MarriageEventRepository
                    .GetAll().Where(m => m.Id == request.Id)
                    .Include(m => m.BrideInfo)
                    .ThenInclude(b => b.ContactInfo)
                    .Include(m => m.Event)
                    .Include(m => m.Event.EventOwener).ThenInclude(e => e.ContactInfo)
                    .Include(m => m.Event.EventSupportingDocuments)
                    .Include(m => m.Witnesses)
                    .Include(m => m.Event.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
                    .ProjectTo<UpdateMarriageEventCommand>(_mapper.ConfigurationProvider)
                    .FirstOrDefault();
            if (MarriageEvent == null)
            {
                throw new NotFoundException($"marriage Event with id {request.Id} not found");
            }
            MarriageEvent.BrideInfo.BirthAddressResponseDTO = await _AddressService.FormatedAddress(MarriageEvent?.BrideInfo?.BirthAddressId);
            MarriageEvent.BrideInfo.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(MarriageEvent?.BrideInfo?.ResidentAddressId);

            MarriageEvent.Event.EventOwener.BirthAddressResponseDTO = await _AddressService.FormatedAddress(MarriageEvent?.Event.EventOwener?.BirthAddressId);
            MarriageEvent.Event.EventOwener.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(MarriageEvent?.Event.EventOwener?.ResidentAddressId);

            MarriageEvent.Event.EventAddressResponseDTO = await _AddressService.FormatedAddress(MarriageEvent?.Event.EventAddressId);

            foreach (var wit in MarriageEvent.Witnesses)
            {
                wit.WitnessPersonalInfo.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(wit?.WitnessPersonalInfo?.ResidentAddressId);
            }
            var ids = new List<string?>{
                MarriageEvent!.BrideInfo?.Id.ToString(),
                MarriageEvent.Event?.EventOwener?.Id.ToString(),
            };
            ids.AddRange(MarriageEvent.Witnesses.Select(w => w.WitnessPersonalInfo.Id.ToString()));
            MarriageEvent.Event.fingerPrints = _eventDocumentService.getFingerprintUrls(ids.Where(id => id != null).ToList()!);

            MarriageEvent.Event.fingerPrints = new
            {
                Husband = _eventDocumentService.getSingleFingerprintUrls(MarriageEvent.Event.EventOwener?.Id.ToString()),
                Wife = _eventDocumentService.getSingleFingerprintUrls(MarriageEvent.BrideInfo
                ?.Id.ToString())
            };
            var witnessFingerPrints =

            MarriageEvent.Event.fingerPrints = new
            {
                Husband = _eventDocumentService.getSingleFingerprintUrls(MarriageEvent.Event.EventOwener?.Id.ToString()),
                Wife = _eventDocumentService.getSingleFingerprintUrls(MarriageEvent.BrideInfo?.Id.ToString()),
                Witness = MarriageEvent.Witnesses.Select(w => _eventDocumentService.getSingleFingerprintUrls(w.WitnessPersonalInfo?.Id.ToString()).ToList())
            };
            if (request.TransactionId is not null)
            {
                MarriageEvent.Comment = await _mediator.Send(new GetNotificationByTransactionIdQuery { Id = (Guid)request.TransactionId });
            }

            return MarriageEvent;
        }
    }
}