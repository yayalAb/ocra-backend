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
    public class SystemAuditQueryHandler : IRequestHandler<SystemAuditQuery, PaginatedList<SystemAuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditService;
        private readonly IEventRepository _eventRepository;

        public SystemAuditQueryHandler(IAuditLogRepository auditLogRepository, IUserRepository userRepository, IAuditLogService auditService, IEventRepository eventRepository)
        {
            this._userRepository = userRepository;
            this._auditService = auditService;
            this._eventRepository = eventRepository;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<SystemAuditGridDTO>> Handle(SystemAuditQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAllGrid().Where(a => a.EntityType == "Lookup" || a.EntityType == "Address" || a.EntityType == "Setting");
            
            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new SystemAuditGridDTO(a, _userRepository))
                        .PaginateAsync<SystemAuditGridDTO,SystemAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
