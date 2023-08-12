using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    // Customer query with List<Customer> response
    public record thisMonthAuthenticatedCertificateList : IRequest<PaginatedList<PaidCertificateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class thisMonthAuthenticatedCertificateListHandler : IRequestHandler<thisMonthAuthenticatedCertificateList, PaginatedList<PaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReturnVerficationList _ReturnVerficationList;
        private readonly IUserResolverService _UserReso;

        public thisMonthAuthenticatedCertificateListHandler(IUserResolverService UserReso, IReturnVerficationList ReturnVerficationList, IUserRepository userRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _ReturnVerficationList = ReturnVerficationList;
            _UserReso = UserReso;
        }
        public async Task<PaginatedList<PaidCertificateDTO>> Handle(thisMonthAuthenticatedCertificateList request, CancellationToken cancellationToken)
        {
            var applicationuser = _userRepository.GetAll()
            .Include(x => x.Address)
            .Where(x => x.PersonalInfoId == _UserReso.GetUserPersonalId()).FirstOrDefault();
            if (applicationuser == null)
            {
                throw new NotFoundException("user does not exist");
            }
            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Include(x=>x.EventCertificates)
                              .Where(e => e.EventRegisteredAddressId == applicationuser.AddressId|| e.CreatedBy==new Guid(applicationuser.Id));
            IQueryable<Event> eventsQueriable= eventByCivilReg.Where(e => e.EventCertificates
            .Where(s=>s.Status && s.ModifiedAt<DateTime.Now.AddDays(30)).FirstOrDefault().AuthenticationStatus);
            eventsQueriable = eventsQueriable.Include(e => e.EventOwener);
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                eventsQueriable = eventsQueriable.Where(
                    u => EF.Functions.Like(u.CertificateId, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventType, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventDateEt!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventRegDateEt, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.EventOwener.LastNameStr!, "%" + request.SearchString + "%"));
            }

            return await PaginatedList<PaidCertificateDTO>
                            .CreateAsync(
                               eventsQueriable.Include(e => e.EventOwener)
                             .Include(e => e.EventPaymentRequest).OrderBy(x => x.ModifiedAt)

                              .Select(e => new PaidCertificateDTO
                              {
                                  EventId = e.Id,
                                  CertificateId = e.CertificateId,
                                  EventType = e.EventType,
                                  OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                                  EventDate = e.EventDateEt,
                                  EventRegDate = e.EventRegDateEt,
                                  IsCertified = e.IsCertified,
                                  HasPendingDocumentApproval = e.HasPendingDocumentApproval,
                                  IsReprint = e.ReprintWaiting,
                              })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}