using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using MediatR;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public record SystemAuditQuery : IRequest<PaginatedList<SystemAuditGridDTO>>
    {
        public Guid? AddressId { get; set; }
        public string? UserId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? EntityType { get; set; }
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }
}
