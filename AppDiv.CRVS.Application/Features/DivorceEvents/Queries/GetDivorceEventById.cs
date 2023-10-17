
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Features.DivorceEvents.Command.Update;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Notifications.Queries.GetNotificationByTransactionId;
using AppDiv.CRVS.Utility.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.DivorceEvents.Query
{
    // Customer query with List<Customer> response
    public record GetDivorceEventByIdQuery : IRequest<UpdateDivorceEventCommand>
    {
        public Guid Id { get; set; }
    }

    public class GetDivorceEventByIdQueryHandler : IRequestHandler<GetDivorceEventByIdQuery, UpdateDivorceEventCommand>
    {
        private readonly IDivorceEventRepository _DivorceEventRepository;
        private readonly IDateAndAddressService _AddressService;

        private readonly IMapper _mapper;
        private readonly IEventDocumentService _eventDocumentService;
        private readonly IMediator _mediator;

        public GetDivorceEventByIdQueryHandler(
            IDivorceEventRepository DivorceEventRepository, 
            IDateAndAddressService AddressService, 
            IMapper mapper, 
            IEventDocumentService eventDocumentService,
            IMediator mediator)
        {
            _DivorceEventRepository = DivorceEventRepository;
            _AddressService = AddressService;
            _mapper = mapper;
            _eventDocumentService = eventDocumentService;
            this._mediator = mediator;
        }
        public async Task<UpdateDivorceEventCommand> Handle(GetDivorceEventByIdQuery request, CancellationToken cancellationToken)
        {

            var DivorceEvent = await _DivorceEventRepository
                    .GetAll().Where(m => m.Id == request.Id)
                    .Include(m => m.CourtCase)
                    .Include(m => m.DivorcedWife)
                    .ThenInclude(b => b.ContactInfo)
                    .Include(m => m.Event)
                    .Include(m => m.Event.EventOwener).ThenInclude(e => e.ContactInfo)
                    .Include(m => m.Event.EventSupportingDocuments)
                    .Include(m => m.Event.PaymentExamption).ThenInclude(p => p.SupportingDocuments)
                    .ProjectTo<UpdateDivorceEventCommand>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);
            if (DivorceEvent == null)
            {
                throw new NotFoundException($"divorce Event with id {request.Id} not found");
            }
            DivorceEvent.DivorcedWife.BirthAddressResponseDTO = await _AddressService.FormatedAddress(DivorceEvent?.DivorcedWife?.BirthAddressId);
            DivorceEvent.DivorcedWife.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(DivorceEvent?.DivorcedWife?.ResidentAddressId);

            DivorceEvent.Event.EventOwener.BirthAddressResponseDTO = await _AddressService.FormatedAddress(DivorceEvent?.Event.EventOwener?.BirthAddressId);
            DivorceEvent.Event.EventOwener.ResidentAddressResponseDTO = await _AddressService.FormatedAddress(DivorceEvent?.Event.EventOwener?.ResidentAddressId);

            DivorceEvent.CourtCase.Court.AddressResponseDTO = await _AddressService.FormatedAddress(DivorceEvent?.CourtCase.Court?.AddressId);

            DivorceEvent.Event.EventAddressResponseDTO = await _AddressService.FormatedAddress(DivorceEvent?.Event.EventAddressId);

            DivorceEvent.Event.fingerPrints = new
            {
                Husband = _eventDocumentService.getSingleFingerprintUrls(DivorceEvent.Event.EventOwener?.Id.ToString()),
                Wife = _eventDocumentService.getSingleFingerprintUrls(DivorceEvent.DivorcedWife?.Id.ToString())
            };
            return DivorceEvent;
        }
    }
}