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
            // var correctionRequestList = _correctionRequestRepo.GetAll().Where(x => x.Request.CivilRegOfficerId == request.CivilRegOfficerId);
            // List<Event> CorrectionRequestList = new List<Event>();
            // var eventList = correctionRequestList.Select(x => x.Content);//.FirstOrDefault();
            // foreach (JObject evn in eventList)
            // {
            //     Event EventList = evn.Value<JObject>("event").ToObject<Event>();
            //     CorrectionRequestList.Add(EventList);
            // }
            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Include(e => e.EventOwener)
                              .Where(e => e.CivilRegOfficerId == request.CivilRegOfficerId && !e.IsCertified);

            return PaginatedList<PaidCertificateDTO>
                            .CreateAsync(
                               eventByCivilReg
                              .Select(e => new PaidCertificateDTO
                              {
                                  EventId = e.Id,
                                  CertificateId = e.CertificateId,
                                  EventType = e.EventType,
                                  OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                                  EventDate = e.EventDate,
                                  EventRegDate = e.EventRegDate,
                                  IsCertified = e.IsCertified
                              })
                              .ToList()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}