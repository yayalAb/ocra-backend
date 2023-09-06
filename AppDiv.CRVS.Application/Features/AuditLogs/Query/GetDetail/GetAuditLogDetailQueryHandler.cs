using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class GetAuditLogDetailQueryHandler : IRequestHandler<GetAuditLogDetailQuery, object>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAuditLogService _auditService;
        private readonly IMediator _mediator;

        public GetAuditLogDetailQueryHandler(IAuditLogRepository auditLogRepository, IAuditLogService auditService, IMediator mediator)
        {
            _auditLogRepository = auditLogRepository;
            this._auditService = auditService;
            this._mediator = mediator;
        }
        public async Task<object> Handle(GetAuditLogDetailQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAll().Where(a => a.AuditId == request.Id).FirstOrDefault();
            // var response = _mediator.Send(new GetAuditLogQuery());
            var data = new JObject();
            var eventType =  audit!.EntityType.EndsWith("Event") ? audit.EntityType[..^5] : audit.EntityType;
            var newData = await _mediator.Send(new GenerateArchivePreviewQuery { Content = _auditService.GetAuditArchive(audit?.AuditDataJson?.Value<JObject>("ColumnValues")), EventType = eventType, Command = "Update"}, cancellationToken);
            var oldData = audit?.AuditDataJson?.Value<string>("Action") == "Insert" ? null : await _mediator.Send(new GenerateArchivePreviewQuery { Content = _auditService.GetAuditArchive(_auditService.GetContent(audit?.AuditDataJson?.Value<JArray>("Changes"))), EventType = eventType, Command = "Update"}, cancellationToken);
            var result = 
                new 
                { 
                    NewData = (newData as dynamic).Content, 
                    OldData =  (oldData as dynamic)?.Content,
                    newVal = _auditService.GetAuditArchive(audit?.AuditDataJson?.Value<JObject>("ColumnValues")),
                    // OldValue = _auditService.GetNestedElements(_auditService.GetContent(audit?.AuditDataJson?.Value<JArray>("Changes"))),
                    // NewValue = _auditService.GetNestedElements(audit?.AuditDataJson?.Value<JObject>("ColumnValues")),
                };
            // data["NewValues"] = await _mediator.Send(new GenerateArchivePreviewQuery { Content = _auditService.GetNestedElements(audit?.AuditDataJson?.Value<JObject>("ColumnValues")), EventType = audit.EntityType, Command = "Update"});
            // data["Changes"] = _auditService.GetAllChanges(audit.AuditDataJson.Value<JArray>("Changes"), audit.AuditDate, audit.EntityType);
            // if (audit?.Action == "Update")
            // {
            //     data["OldValues"] = _auditService.GetNestedElements(_auditService.GetContent(audit?.AuditDataJson?.Value<JArray>("Changes")));
            // }
            return result;
        }
    }
}
