using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    // Customer query with List<Customer> response
    public record GetAllPaidCertificateByCivilRegistrarQuery : IRequest<PaginatedList<PaidCertificateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public Guid CivilRegOfficerId { get; set; }
        public bool isVerification { get; set; } = false;
        public string? SearchString { get; set; }
    }

    public class GetAllPaidCertificateByCivilRegistrarQueryHandler : IRequestHandler<GetAllPaidCertificateByCivilRegistrarQuery, PaginatedList<PaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReturnVerficationList _ReturnVerficationList;

        public GetAllPaidCertificateByCivilRegistrarQueryHandler(IReturnVerficationList ReturnVerficationList, IUserRepository userRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _ReturnVerficationList = ReturnVerficationList;
        }
        public async Task<PaginatedList<PaidCertificateDTO>> Handle(GetAllPaidCertificateByCivilRegistrarQuery request, CancellationToken cancellationToken)
        {
            var applicationuser = _userRepository.GetAll()
            .Include(x => x.Address)
            .Where(x => x.PersonalInfoId == request.CivilRegOfficerId).FirstOrDefault();
            if (applicationuser == null)
            {
                throw new Exception("user does not exist");
            }
            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Where(e => e.CivilRegOfficerId == request.CivilRegOfficerId || e.ReprintWaiting);
            IQueryable<Event> eventsQueriable;
            if (request.isVerification)
            {
                eventsQueriable = await _ReturnVerficationList.GetVerficationRequestedCertificateList(request.CivilRegOfficerId);
            }
            else
            {
                eventsQueriable = eventByCivilReg.Where(e => (!e.IsCertified && (e.IsPaid || e.IsExampted) || (e.ReprintWaiting)));
            }
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
                             .Include(e => e.EventPaymentRequest)
                                .ThenInclude(e => e.Payment)
                              .Select(e => new PaidCertificateDTO
                              {
                                  EventId = e.Id,
                                  CertificateId = e.CertificateId,
                                  EventType = e.EventType,
                                  OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                                  EventDate = e.EventDateEt,
                                  EventRegDate = e.EventRegDateEt,
                                  IsCertified = e.IsCertified,
                                  IsReprint = e.ReprintWaiting,
                                  PaymentDate = e.EventPaymentRequest
                                        .Where(r => r.PaymentRate.PaymentTypeLookup.ValueStr.ToLower().Contains("certificategeneration")
                                        || r.PaymentRate.PaymentTypeLookup.ValueStr.ToLower().Contains("reprint"))
                                        .FirstOrDefault().Payment.CreatedAt

                              }).OrderByDescending(e => e.PaymentDate)
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}