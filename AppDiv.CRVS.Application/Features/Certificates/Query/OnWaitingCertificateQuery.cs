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
        private readonly IUserResolverService _userResolverService;

        public OnWaitingCertificateQueryHandler(ISettingRepository SettinglookupRepository, IUserResolverService userResolverService, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _SettinglookupRepository = SettinglookupRepository;
            _userResolverService = userResolverService;
        }
        public async Task<PaginatedList<PaidCertificateDTO>> Handle(OnWaitingCertificateQuery request, CancellationToken cancellationToken)
        {
            var generalSetting = _SettinglookupRepository.GetAll().Where(x => x.Key == "generalSetting").FirstOrDefault();
            if (generalSetting == null)
            {
                throw new NotFoundException("General Setting Does not found");
            }
            int editWaitingTime = int.Parse(generalSetting.Value.Value<string>("edit_wating_time"));
             if (editWaitingTime == null)
            {
                throw new NotFoundException("edit waiting time does not Set in general setting");
            }
            Guid? CreatedByCivilId = _userResolverService.GetUserPersonalId();
            Guid? CreatedByUserId = _userResolverService.GetUserPersonalId();
            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Include(x => x.EventCertificates.OrderByDescending(x => x.CreatedAt))
                              .Where(e => ((e.CivilRegOfficerId ==CreatedByCivilId) || (e.CreatedBy == CreatedByUserId)
                              && e.IsCertified) && (e.EventCertificates.Where(x=>x.Status).FirstOrDefault().CreatedAt > DateTime.Now.AddHours(-editWaitingTime)));

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
                              }).OrderByDescending(x => x.CertifiedAt)
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}