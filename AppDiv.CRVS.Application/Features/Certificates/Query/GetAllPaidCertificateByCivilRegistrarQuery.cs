using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
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
    }

    public class GetAllPaidCertificateByCivilRegistrarQueryHandler : IRequestHandler<GetAllPaidCertificateByCivilRegistrarQuery, PaginatedList<PaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public GetAllPaidCertificateByCivilRegistrarQueryHandler(IUserRepository userRepository, IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }
        public async Task<PaginatedList<PaidCertificateDTO>> Handle(GetAllPaidCertificateByCivilRegistrarQuery request, CancellationToken cancellationToken)
        {
            var applicationuser = _userRepository.GetAll()

            .Where(x => x.PersonalInfoId == request.CivilRegOfficerId);
            if (applicationuser == null)
            {
                throw new Exception("user does not exist");
            }
            var eventByCivilReg = _eventRepository.GetAllQueryableAsync()
                              .Where(e => e.CivilRegOfficerId == request.CivilRegOfficerId || e.ReprintWaiting);
            IQueryable<Event> eventsQueriable;
            if (request.isVerification)
            {
                eventsQueriable = eventByCivilReg
                .Include(x => x.CivilRegOfficer)
                .ThenInclude(x => x.ApplicationUser)
                .ThenInclude(a => a.Address)
                .ThenInclude(p => p.ParentAddress)
                .ThenInclude(p => p.ParentAddress)
                .ThenInclude(p => p.ParentAddress)
                .ThenInclude(p => p.ParentAddress)
                .Where(e => e.IsCertified && !e.IsVerified);

            }
            else
            {
                eventsQueriable = eventByCivilReg.Where(e => (!e.IsCertified && (e.IsPaid || e.IsExampted) || (e.ReprintWaiting)));
            }

            return await PaginatedList<PaidCertificateDTO>
                            .CreateAsync(
                               eventsQueriable.Include(e => e.EventOwener)
                              .Select(e => new PaidCertificateDTO
                              {
                                  EventId = e.Id,
                                  CertificateId = e.CertificateId,
                                  EventType = e.EventType,
                                  OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                                  EventDate = e.EventDateEt,
                                  EventRegDate = e.EventRegDateEt,
                                  IsCertified = e.IsCertified,
                                  IsReprint = e.ReprintWaiting
                              })
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}