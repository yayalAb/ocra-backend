

using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Utility.Contracts;
using AppDiv.CRVS.Utility.Services;
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
             var convertor = new CustomDateConverter();

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
                throw new NotFoundException("Certificate with given Id not Found or Certificate Event Not Found");
            }
            var events = _EventRepository.GetAll()
            .Include(x => x.CivilRegOfficer)
            .Include(x => x.CivilRegOfficer.ApplicationUser.UserGroups)
            .Include(x => x.EventPaymentRequest).ThenInclude(x => x.Payment)
            .Include(x => x.EventCertificates)
            .Include(x => x.CorrectionRequests)
            .Where(x => x.Id == selectedEvent.EventId)
            .Select(da => new
            
            {
                Registered = da,
                payment = da.EventPaymentRequest.FirstOrDefault().Payment,
                certificate = da.EventCertificates,
                correction = da.CorrectionRequests
            }).FirstOrDefault();
            if (events == null)
            {
                throw new NotFoundException("The  Certificate Event Is not Exist");
            }

            var listofHistory = _CertificateHistoryRepository.GetAll()
            .Include(x => x.CivilRegOfficer.ApplicationUser.UserGroups)
            .Include(x => x.CivilRegOfficer.ApplicationUser.Address)
            .Where(x => x.CerteficateId == request.Id)
            .Select(h => new EventHistory
            {
                Action = "Reprinted",
                Date = convertor.GregorianToEthiopic(h.CreatedAt),
                By = h.CivilRegOfficer.FirstNameLang + " " + h.CivilRegOfficer.MiddleNameLang + " " + h.CivilRegOfficer.LastNameLang,
                Type = h.CivilRegOfficer.ApplicationUser.UserGroups.Select(x => x.GroupName).FirstOrDefault(),
                Address = h.CivilRegOfficer.ApplicationUser.Address.AddressNameLang
            });
            if (listofHistory != null)
            {
                selectedEvent.Historys = listofHistory.ToList();
            }
            if (events?.Registered != null)
            {
                var history = new EventHistory
                {
                    Action = "Registered",
                    Date = events.Registered.EventDateEt,
                    By = events.Registered.CivilRegOfficer.FirstNameLang + " " + events.Registered.CivilRegOfficer.MiddleNameLang + " " + events.Registered.CivilRegOfficer.LastNameLang,
                    Type = events?.certificate?.FirstOrDefault().Event?.CivilRegOfficer?.ApplicationUser?.UserGroups?.Select(x => x.GroupName)?.FirstOrDefault(),
                    Address = events?.certificate?.FirstOrDefault().Event?.EventAddress?.AddressNameLang,
                };
                if (history == null)
                {
                    throw new NotFoundException("An Error occered on history generatin");
                }
                selectedEvent.Historys.Add(history);
            }
            if (events?.payment != null)
            {
                var history = new EventHistory
                {
                    Action = "paid",
                    Date = convertor.GregorianToEthiopic((DateTime)events?.payment?.CreatedAt),
                    By = events.Registered.CivilRegOfficer.FirstNameLang + " " + events.Registered.CivilRegOfficer.MiddleNameLang + " " + events.Registered.CivilRegOfficer.LastNameLang,
                    Type = events?.certificate?.FirstOrDefault().Event?.CivilRegOfficer?.ApplicationUser?.UserGroups?.Select(x => x.GroupName)?.FirstOrDefault(),
                    Address = events?.certificate?.FirstOrDefault().Event?.EventAddress?.AddressNameLang,

                };
                if (history == null)
                {
                    throw new NotFoundException("An Error occered on history generatin");
                }
                selectedEvent.Historys.Add(history);
            }

            if (events?.certificate != null)
            {
                var history = new EventHistory
                {
                    Action = "certificate",
                    Date = convertor.GregorianToEthiopic((DateTime)events?.certificate?.FirstOrDefault().CreatedAt),
                    By = events.Registered.CivilRegOfficer.FirstNameLang + " " + events.Registered.CivilRegOfficer.MiddleNameLang + " " + events.Registered.CivilRegOfficer.LastNameLang,
                    Type = events?.certificate?.FirstOrDefault().Event?.CivilRegOfficer?.ApplicationUser?.UserGroups?.Select(x => x.GroupName)?.FirstOrDefault(),
                    Address = events?.certificate?.FirstOrDefault().Event?.EventAddress?.AddressNameLang,

                };
                if (history == null)
                {
                    throw new NotFoundException("An Error occered on history generatin");
                }
                selectedEvent.Historys.Add(history);
            }

            return CustomMapper.Mapper.Map<EventHistoryDto>(selectedEvent);
        }
    }
}