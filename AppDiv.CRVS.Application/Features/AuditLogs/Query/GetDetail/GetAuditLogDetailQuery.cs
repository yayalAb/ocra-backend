using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public record GetAuditLogDetailQuery : IRequest<JObject>
    {
        public Guid Id { set; get; }
    }
}
