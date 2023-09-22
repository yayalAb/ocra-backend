using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class SystemAuditDetailQueryHandler : IRequestHandler<SystemAuditDetailQuery, object>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAuditLogService _auditService;
        private readonly IMediator _mediator;

        public SystemAuditDetailQueryHandler(IAuditLogRepository auditLogRepository, IAuditLogService auditService, IMediator mediator)
        {
            _auditLogRepository = auditLogRepository;
            this._auditService = auditService;
            this._mediator = mediator;
        }
        public async Task<object> Handle(SystemAuditDetailQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAll().Where(a => a.AuditId == request.Id).FirstOrDefault();
            
            var result = 
                new 
                {
                    OldValue = audit?.Action == "Insert" ? null : _auditService.GetNestedElements(_auditService.GetContent(audit?.AuditDataJson)),
                    NewValue = _auditService.GetNestedElements(audit?.AuditDataJson?.Value<JObject>("ColumnValues")),
                };
            return result;
        }
    }
}
