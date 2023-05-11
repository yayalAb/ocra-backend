using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
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
    }

    public class GetAllPaidCertificateByCivilRegistrarQueryHandler : IRequestHandler<GetAllPaidCertificateByCivilRegistrarQuery, PaginatedList<PaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;

        public GetAllPaidCertificateByCivilRegistrarQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<PaginatedList<PaidCertificateDTO>> Handle(GetAllPaidCertificateByCivilRegistrarQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<PaidCertificateDTO>
                            .CreateAsync(
                             await _eventRepository.GetAllQueryableAsync()
                              .Where(e =>e.CivilRegOfficerId == request.CivilRegOfficerId &&(e.IsPaid || e.IsExampted) && !e.IsCertified)
                              .Include(e => e.EventOwener)
                              .Select(e => new PaidCertificateDTO{
                                EventId = e.Id,
                                CertificateId = e.CertificateId,
                                EventType = e.EventType,
                                OwnerFullName = e.EventOwener.FirstNameLang + " "+e.EventOwener.MiddleNameLang + " "+e.EventOwener.LastNameLang,
                                EventDate = e.EventDate,
                                EventRegDate = e.EventRegDate,
                                IsCertified = e.IsCertified
                              })
                              .ToListAsync()
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}