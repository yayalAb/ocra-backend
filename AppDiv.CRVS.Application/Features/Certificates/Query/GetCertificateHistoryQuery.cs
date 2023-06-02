

using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
        public Guid Id { get; set; }
    }

    public class GetCertificateHistoryQueryHandler : IRequestHandler<GetCertificateHistoryQuery, EventHistoryDto>
    {
        private readonly IEventRepository _EventRepository;
        private readonly ICertificateRepository _CertificateRepository;
        private readonly ICertificateHistoryRepository _CertificateHistoryRepository;

        public GetCertificateHistoryQueryHandler(IEventRepository EventRepository, ICertificateRepository CertificateRepository,
        ICertificateHistoryRepository CertificateHistoryRepository)
        {
            _EventRepository = EventRepository;
            _CertificateRepository = CertificateRepository;
            _CertificateHistoryRepository = CertificateHistoryRepository;
        }
        public async Task<EventHistoryDto> Handle(GetCertificateHistoryQuery request, CancellationToken cancellationToken)
        {

            var selectedEvent = _CertificateRepository.GetAll()
            .Where(x => x.Id == request.Id)
            .Select(x => new EventHistoryDto
            {
                EventOwner = x.Event.EventOwener.FirstNameLang + " " + x.Event.EventOwener.MiddleNameLang + " " + x.Event.EventOwener.LastNameLang,
                Status = x.Event.IsCertified ? "Certified" : "Not Certified",
                Event = x.Event.EventType,
                Informant = x.Event.InformantType,
                CerificateId = x.Event.CertificateId,
                EventId = x.Event.Id,
                EventDate = x.Event.EventDateEt,
                EventAddress = x.Event.EventAddress.AddressNameLang
            }).FirstOrDefault();
            if (selectedEvent == null)
            {
                throw new Exception("Certificate with given Id not Found");
            }
            var events = _EventRepository.GetAll()
            .Include(x => x.CivilRegOfficer)
            .Include(x => x.CivilRegOfficer.ApplicationUser.UserGroups)
            .Include(x => x.EventPaymentRequest.Payment)
            .Include(x => x.EventCertificates)
            .Include(x => x.CorrectionRequests)
            .Where(x => x.Id == selectedEvent.EventId)
            .Select(da => new
            {
                Registered = da,
                payment = da.EventPaymentRequest.Payment,
                certificate = da.EventCertificates,
                correction = da.CorrectionRequests
            }).FirstOrDefault();
            var listofHistory = _CertificateHistoryRepository.GetAll()
            .Include(x => x.CivilRegOfficer.ApplicationUser.UserGroups)
            .Where(x => x.CerteficateId == request.Id)
            .Select(h => new EventHistory
            {
                Action = "Reprinted",
                Date = h.CreatedAt.ToString(),
                By = h.CivilRegOfficer.FirstNameLang + " " + h.CivilRegOfficer.MiddleNameLang + " " + h.CivilRegOfficer.LastNameLang,
                Type = h.CivilRegOfficer.ApplicationUser.UserGroups.ToString(),
                Address = h.CivilRegOfficer.ApplicationUser.Address.AddressNameLang
            });
            selectedEvent.Historys = listofHistory.ToList();

            var history = new EventHistory
            {
                Action = "Registered",
                Date = events.Registered.EventDateEt,
                By = events.Registered.CivilRegOfficer.FirstNameLang + " " + events.Registered.CivilRegOfficer.MiddleNameLang + " " + events.Registered.CivilRegOfficer.LastNameLang,
                Type = events.Registered.CivilRegOfficer.ApplicationUser.UserGroups.ToString(),
                Address = events.Registered.EventAddress.AddressNameLang
            };
            if (history == null)
            {
                throw new Exception("An Error occered on history generatin");
            }
            selectedEvent.Historys.Append(history);
            return CustomMapper.Mapper.Map<EventHistoryDto>(selectedEvent);
        }
    }
}