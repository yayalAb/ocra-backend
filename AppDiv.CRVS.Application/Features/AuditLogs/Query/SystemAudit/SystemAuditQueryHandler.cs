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
    public class SystemAuditQueryHandler : IRequestHandler<SystemAuditQuery, PaginatedList<AuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditService;

        public SystemAuditQueryHandler(IAuditLogRepository auditLogRepository, IUserRepository userRepository, IAuditLogService auditService)
        {
            this._userRepository = userRepository;
            this._auditService = auditService;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<AuditGridDTO>> Handle(SystemAuditQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAll().Where(a => a.EntityType == "Lookup" || a.EntityType == "Address" || a.EntityType == "Setting");
            
            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new AuditGridDTO(a, request.WithContent, _userRepository))
                        .PaginateAsync<AuditGridDTO,AuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
