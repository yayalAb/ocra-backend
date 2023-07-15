using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IAuditLogService
    {
        JObject GetContent(JArray? content);
        JObject GetNestedElements(JObject? content);
    }
}