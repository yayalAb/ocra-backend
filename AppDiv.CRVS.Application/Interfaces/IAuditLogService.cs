using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain.Entities.Audit;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IAuditLogService
    {
        JObject GetContent(JObject? content);
        JObject GetNestedElements(JObject? content);
        JArray GetAllChanges(JArray content, DateTime auditDate, string entityType);
        JObject GetAuditArchive(JObject content);
        JObject EventAudit(AuditLog audit);
    }
}