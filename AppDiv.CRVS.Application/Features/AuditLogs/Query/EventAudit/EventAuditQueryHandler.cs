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
    public class EventAuditQueryHandler : IRequestHandler<EventAuditQuery, PaginatedList<EventAuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditService;
        private readonly IEventRepository _eventRepository;

        public EventAuditQueryHandler(IAuditLogRepository auditLogRepository, IAuditLogService auditService, IEventRepository eventRepository)
        {
            this._auditService = auditService;
            this._eventRepository = eventRepository;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<EventAuditGridDTO>> Handle(EventAuditQuery request, CancellationToken cancellationToken)
        {
            var events = new List<string> { "BirthEvent", "DeathEvent", "MarriageEvent", "DivorceEvent", "AdoptionEvent" };
            var audit = _auditLogRepository.GetAllGrid().Where(a => events.Contains(a.EntityType));
            
            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new EventAuditGridDTO(a, _eventRepository))
                        .PaginateAsync<EventAuditGridDTO,EventAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
