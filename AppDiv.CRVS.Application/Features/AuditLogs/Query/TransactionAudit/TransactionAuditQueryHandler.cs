using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;
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
            var audit = _transactionService.GetAllGrid();
            
            return await audit.OrderByDescending(t => t.Request.CreatedAt).Select(t => new TransactionAuditGridDTO(t))
                        .PaginateAsync<TransactionAuditGridDTO,TransactionAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
