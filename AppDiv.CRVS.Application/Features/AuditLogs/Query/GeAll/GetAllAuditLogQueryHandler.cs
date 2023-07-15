using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class GetAllAuditLogQueryHandler : IRequestHandler<GetAllAuditLogQuery, PaginatedList<AuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public GetAllAuditLogQueryHandler(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<AuditGridDTO>> Handle(GetAllAuditLogQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAll();
            if (request.Id != null)
            {
                audit = audit.Where(a => request.Id == a.TablePk);
            }
            

            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new AuditGridDTO(a, request.WithContent))
                        .PaginateAsync<AuditGridDTO,AuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
