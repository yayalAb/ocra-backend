using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    // Customer query with List<Customer> response
    public record EventDraftListQuery : IRequest<PaginatedList<PaidCertificateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public Guid CivilRegOfficerId { get; set; }
        public string? SearchString { get; set; }
    }

    public class EventDraftListQueryHandler : IRequestHandler<EventDraftListQuery, PaginatedList<PaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICorrectionRequestRepostory _correctionRequestRepo;

        public EventDraftListQueryHandler(IEventRepository eventRepository, ICorrectionRequestRepostory correctionRequestRepo)
        {
            _eventRepository = eventRepository;
            _correctionRequestRepo = correctionRequestRepo;
        }
        public Task<PaginatedList<PaidCertificateDTO>> Handle(EventDraftListQuery request, CancellationToken cancellationToken)
        {
            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Include(e => e.EventOwener)
                              .Include(x => x.BirthEvent)
                              .Include(x => x.AdoptionEvent)
                              .Include(x => x.MarriageEvent)
                              .Include(x => x.DivorceEvent)
                              .Include(x => x.DeathEventNavigation)
                              .Where(e => e.CivilRegOfficerId == request.CivilRegOfficerId && !e.IsCertified);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                eventByCivilReg = eventByCivilReg.Where(
                    u => EF.Functions.Like(u.EventOwener.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.LastNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventType, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventDateEt!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventRegDateEt, "%" + request.SearchString + "%"));
            }
            return PaginatedList<PaidCertificateDTO>
                            .CreateAsync(
                               eventByCivilReg
                              .Select(e => new PaidCertificateDTO
                              {
                                  EventId = e.BirthEvent == null ? e.DeathEventNavigation == null ? e.AdoptionEvent == null ?
                                  e.MarriageEvent == null ? e.DivorceEvent == null ? Guid.Empty : e.DivorceEvent.Id : e.MarriageEvent.Id :
                                  e.AdoptionEvent.Id : e.DeathEventNavigation.Id : e.BirthEvent.Id,
                                  CertificateId = e.CertificateId,
                                  EventType = e.EventType,
                                  OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                                  EventDate = e.EventDateEt,
                                  EventRegDate = e.EventRegDateEt,
                                  IsCertified = e.IsCertified
                              })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}