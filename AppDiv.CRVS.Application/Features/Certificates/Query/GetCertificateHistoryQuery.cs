

using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Customers.Query
{
    // Customer GetCustomerByIdQuery with Customer response
    public class GetCertificateHistoryQuery : IRequest<EventHistoryDto>
    {
        public Guid Id { get; private set; }

        public GetCertificateHistoryQuery(Guid Id)
        {
            this.Id = Id;
        }
    }

    public class GetCertificateHistoryQueryHandler : IRequestHandler<GetCertificateHistoryQuery, EventHistoryDto>
    {
        private readonly IEventRepository _EventRepository;
        private readonly ICertificateRepository _CertificateRepository;

        public GetCertificateHistoryQueryHandler(IEventRepository EventRepository, ICertificateRepository CertificateRepository)
        {
            _EventRepository = EventRepository;
            _CertificateRepository = CertificateRepository;
        }
        public async Task<EventHistoryDto> Handle(GetCertificateHistoryQuery request, CancellationToken cancellationToken)
        {
            // var customers = await _mediator.Send(new GetAllCustomerQuery());
            var explicitLoadedProperties = new Dictionary<string, Utility.Contracts.NavigationPropertyType>
                                                {
                                                    { "Event", NavigationPropertyType.REFERENCE }

                                                };

            var selectedEvent = _CertificateRepository.GetAll().Where(x => x.Id == request.Id)
            .Select(x => new EventHistoryDto
            {
                EventOwner = x.Event.EventOwener.FirstNameLang + " " + x.Event.EventOwener.MiddleNameLang + " " + x.Event.EventOwener.LastNameLang,
                Status = x.Event.IsCertified ? "Certified" : "Not Certified",
                Event = x.Event.EventType,
                Informant = x.Event.InformantType,
                EventId = x.Event.CertificateId,
                EventDate = x.Event.EventDateEt,
                EventAddress = x.Event.EventAddress.AddressNameLang
            });
            if (selectedEvent == null)
            {
                throw new Exception("Certificate with given Id not Found");
            }
            return CustomMapper.Mapper.Map<EventHistoryDto>(selectedEvent);
        }
    }
}