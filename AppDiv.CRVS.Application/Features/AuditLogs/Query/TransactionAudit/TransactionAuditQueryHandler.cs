using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class TransactionAuditQueryHandler : IRequestHandler<TransactionAuditQuery, PaginatedList<TransactionAuditGridDTO>>
    {
        private readonly ITransactionService _transactionService;

        public TransactionAuditQueryHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task<PaginatedList<TransactionAuditGridDTO>> Handle(TransactionAuditQuery request, CancellationToken cancellationToken)
        {
            var transactions = _transactionService.GetAllGrid();
            if(request.SearchString is not null)
            {
                transactions = transactions.Where(t => EF.Functions.Like(t.Request!.CivilRegOfficer!.FirstNameStr!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request!.CivilRegOfficer!.MiddleNameStr!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request!.CivilRegOfficer!.LastNameStr!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.CivilRegOfficer!.PersonalInfo.FirstNameStr!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.CivilRegOfficer!.PersonalInfo.MiddleNameStr!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.CivilRegOfficer!.PersonalInfo.LastNameStr!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request.RequestType, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request!.AuthenticationRequest!.Certificate!.Event!.CertificateId!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request!.CorrectionRequest!.Event.CertificateId!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request!.VerficationRequest!.Event.CertificateId!, "%" + request.SearchString + "%")
                                                    || EF.Functions.Like(t.Request!.PaymentRequest!.Event.CertificateId!, "%" + request.SearchString + "%")
                                            );
            }
            if (request.StartDate != null && request.EndDate != null)
            {
                var convertor = new CustomDateConverter();
                var startDate = convertor.EthiopicToGregorian(request.StartDate);
                var endDate = convertor.EthiopicToGregorian(request.EndDate);
                transactions = transactions.Where(a => a.Request.CreatedAt >= startDate && endDate <= a.CreatedAt);
            }
            
            return await transactions.OrderByDescending(t => t.Request!.CreatedAt).Select(t => new TransactionAuditGridDTO(t))
                        .PaginateAsync<TransactionAuditGridDTO,TransactionAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
