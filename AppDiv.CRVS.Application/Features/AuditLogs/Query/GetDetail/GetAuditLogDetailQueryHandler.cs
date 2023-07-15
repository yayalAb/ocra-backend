using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class GetAuditLogDetailQueryHandler : IRequestHandler<GetAuditLogDetailQuery, JObject>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAuditLogService _auditService;

        public GetAuditLogDetailQueryHandler(IAuditLogRepository auditLogRepository, IAuditLogService auditService)
        {
            _auditLogRepository = auditLogRepository;
            this._auditService = auditService;
        }
        public Task<JObject> Handle(GetAuditLogDetailQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAll().Where(a => a.AuditId == request.Id).FirstOrDefault();
            // var response = _mediator.Send(new GetAuditLogQuery());
            var data = new JObject { ["NewValues"] = _auditService.GetNestedElements(audit?.AuditDataJson?.Value<JObject>("ColumnValues")) };
            if (audit?.Action == "Update")
            {
                data["OldValues"] = _auditService.GetNestedElements(_auditService.GetContent(audit?.AuditDataJson?.Value<JArray>("Changes")));
            }
            return Task.FromResult(data);
        }
    }
}
