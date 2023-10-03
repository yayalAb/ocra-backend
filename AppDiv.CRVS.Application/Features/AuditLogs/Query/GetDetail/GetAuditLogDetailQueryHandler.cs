using AppDiv.CRVS.Application.Features.Archives.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var audit = _auditLogRepository.GetAll()
                .Include(a => a.AuditUser.PersonalInfo)
                .Where(a => a.AuditId == request.Id)
                .FirstOrDefault();
            // var response = _mediator.Send(new GetAuditLogQuery());
            var eventType =  audit!.EntityType.EndsWith("Event") ? audit.EntityType[..^5] : audit.EntityType;
            var data = _auditService.EventAudit(audit);

            var newData = await _mediator.Send(new GenerateArchivePreviewQuery { Content = data?.Value<JObject>("newData")!, EventType = eventType, Command = "Update"}, cancellationToken);
            var oldData = audit?.AuditDataJson?.Value<string>("Action") == "Insert" ? null : await _mediator.Send(new GenerateArchivePreviewQuery { Content = data?.Value<JObject?>("oldData")!, EventType = eventType, Command = "Update"}, cancellationToken);
            var result = 
                new 
                {
                    NewData = (newData as dynamic).Content, 
                    OldData =  (oldData as dynamic)?.Content,
                    UserId = audit.AuditUserId,
                    UserFullName = audit.AuditUser.PersonalInfo.FullNameLang
                    // Values = _auditService.EventAudit(audit),
                };
            return result;
        }
    }
}
