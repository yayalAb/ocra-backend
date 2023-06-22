using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.Certificates.Query
{
    public record GetAllUnPaidCertificateByCivilRegistrarQuery : IRequest<PaginatedList<UnPaidCertificateDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public Guid CivilRegOfficerId { get; set; }
    }

    public class GetAllUnPaidCertificateByCivilRegistrarQueryHandler : IRequestHandler<GetAllUnPaidCertificateByCivilRegistrarQuery, PaginatedList<UnPaidCertificateDTO>>
    {
        private readonly IEventRepository _eventRepository;

        public GetAllUnPaidCertificateByCivilRegistrarQueryHandler(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<PaginatedList<UnPaidCertificateDTO>> Handle(GetAllUnPaidCertificateByCivilRegistrarQuery request, CancellationToken cancellationToken)
        {
            return await PaginatedList<UnPaidCertificateDTO>
                        .CreateAsync(
                            _eventRepository.GetAllQueryableAsync()
                           .Where(e => e.CivilRegOfficerId == request.CivilRegOfficerId && !e.IsPaid && !e.IsExampted && !e.IsCertified).Include(e => e.EventOwener).Include(e => e.EventPaymentRequest)
                            .Select(e => new UnPaidCertificateDTO
                            {
                                EventId = e.Id,
                                CertificateId = e.CertificateId,
                                EventType = e.EventType,
                                OwnerFullName = e.EventOwener.FirstNameLang + " " + e.EventOwener.MiddleNameLang + " " + e.EventOwener.LastNameLang,
                                Amount = e.EventPaymentRequest.Amount,
                                PaymentRequestId = e.EventPaymentRequest.Id
                            })
                        , request.PageCount ?? 1, request.PageSize ?? 10);

        }

    }
}