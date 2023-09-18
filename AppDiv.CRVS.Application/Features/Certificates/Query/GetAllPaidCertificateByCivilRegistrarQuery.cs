using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    // Customer query with List<Customer> response
    public record GetAllPaidCertificateByCivilRegistrarQuery : IRequest<PaginatedList<AuthenticationRequestListDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public Guid CivilRegOfficerId { get; set; }
        public bool isVerification { get; set; } = false;
        public string? SearchString { get; set; }
    }

    public class GetAllPaidCertificateByCivilRegistrarQueryHandler : IRequestHandler<GetAllPaidCertificateByCivilRegistrarQuery, PaginatedList<AuthenticationRequestListDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReturnVerficationList _ReturnVerficationList;
        private readonly IUserResolverService _UserReso;

        public GetAllPaidCertificateByCivilRegistrarQueryHandler(IUserResolverService UserReso, IReturnVerficationList ReturnVerficationList, IUserRepository userRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _ReturnVerficationList = ReturnVerficationList;
            _UserReso = UserReso;
        }
        public async Task<PaginatedList<AuthenticationRequestListDTO>> Handle(GetAllPaidCertificateByCivilRegistrarQuery request, CancellationToken cancellationToken)
        {
            var applicationuser = _userRepository.GetAll()
            .Include(x => x.Address)
            .Where(x => x.PersonalInfoId == request.CivilRegOfficerId).FirstOrDefault();
            if (applicationuser == null)
            {
                throw new NotFoundException("user does not exist");
            }
           
            IQueryable<Event> eventsQueriable;
            if (request.isVerification)
            {
                eventsQueriable = await _ReturnVerficationList.GetVerficationRequestedCertificateList(true);
            }
            else
            {   eventsQueriable = _eventRepository.GetAllQueryableAsync()
                                .Include(e => e.CivilRegOfficer)
                              .Where(e =>(e.EventRegisteredAddressId == applicationuser.AddressId|| e.CreatedBy==new Guid(applicationuser.Id))&&((!e.IsCertified && (e.IsPaid || e.IsExampted)) || e.ReprintWaiting));
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

            return await PaginatedList<AuthenticationRequestListDTO>
                            .CreateAsync(
                               eventsQueriable.Include(e => e.EventOwener)
                             .Include(e => e.EventPaymentRequest).OrderBy(x => x.CreatedAt)
                            .Select(e => new AuthenticationRequestListDTO 
                            {
                                Id = e.Id,
                                EventId = e.Id,
                                CertificateId = e.CertificateId,
                                EventType = e.EventType,
                                EventOwnerName = e.EventOwener.FullNameLang,
                                RequestDate = e.EventRegDateEt,
                                RequestedBy = e.CivilRegOfficer.FullNameLang,
                                OfficerId = e.CivilRegOfficerId,
                            })
                            //   .Select(e => new PaidCertificateDTO
                            //   {
                            //       EventId = e.Id,
                            //       CertificateId = e.CertificateId,
                            //       EventType = e.EventType,
                            //       OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                            //       EventDate = e.EventDateEt,
                            //       EventRegDate = e.EventRegDateEt,
                            //       IsCertified = e.IsCertified,
                            //       HasPendingDocumentApproval = e.HasPendingDocumentApproval,
                            //       IsReprint = (e.ReprintWaiting&& (bool)e.IsOfflineReg),
                            //   })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}