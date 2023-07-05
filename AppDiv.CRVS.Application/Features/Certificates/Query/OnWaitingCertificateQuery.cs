using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    // Customer query with List<Customer> response
    public record OnWaitingCertificateQuery : IRequest<PaginatedList<PaidCertificateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public Guid CivilRegOfficerId { get; set; }
        public string? SearchString { get; set; }
    }

    public class OnWaitingCertificateQueryHandler : IRequestHandler<OnWaitingCertificateQuery, PaginatedList<PaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly ISettingRepository _SettinglookupRepository;

        public OnWaitingCertificateQueryHandler(ISettingRepository SettinglookupRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _SettinglookupRepository = SettinglookupRepository;
        }
        public async Task<PaginatedList<PaidCertificateDTO>> Handle(OnWaitingCertificateQuery request, CancellationToken cancellationToken)
        {
            var defualtAddress = _SettinglookupRepository.GetAll().Where(x => x.Key == "generalSetting").FirstOrDefault();
            if (defualtAddress == null)
            {
                throw new NotFoundException("Defualt Address not Found");
            }
            int editWaitingTime = int.Parse(defualtAddress.Value.Value<string>("edit_wating_time"));
            Console.WriteLine("Edit waiting time {0} ", editWaitingTime);


            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Include(x => x.EventCertificates.OrderByDescending(x => x.CreatedAt))
                              .Where(e => (e.CivilRegOfficerId == request.CivilRegOfficerId
                              && e.IsCertified) && (e.EventCertificates.FirstOrDefault().CreatedAt > DateTime.Now.AddHours(-editWaitingTime)));

            eventByCivilReg = eventByCivilReg.Include(e => e.EventOwener);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                eventByCivilReg = eventByCivilReg.Where(
                    u => EF.Functions.Like(u.EventOwener.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.LastNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventType, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventDateEt, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventRegDateEt, "%" + request.SearchString + "%"));
            }
            return await PaginatedList<PaidCertificateDTO>
                            .CreateAsync(
                               eventByCivilReg.Include(e => e.EventOwener)
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
                                  IsCertified = e.IsCertified,
                                  CertifiedAt = e.EventCertificates.OrderByDescending(x => x.CreatedAt).FirstOrDefault().CreatedAt
                              })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}