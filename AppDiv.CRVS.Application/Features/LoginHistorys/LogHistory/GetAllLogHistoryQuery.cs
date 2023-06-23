using System.Text.RegularExpressions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Lookups.Query.GetAllLookup;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Domain.Entities.Audit;
using AppDiv.CRVS.Application.Extensions;

namespace AppDiv.CRVS.Application.Features.LoginHistorys.LogHistory
{
    public class GetAllLogHistoryQuery : IRequest<PaginatedList<AuditLogDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
    }
    public class GetAllLogHistoryQueryHandler : IRequestHandler<GetAllLogHistoryQuery, PaginatedList<AuditLogDTO>>
    {
        private readonly IAuditLogRepository _LoginHistoryRepo;

        public GetAllLogHistoryQueryHandler(IAuditLogRepository LoginHistoryRepo)
        {
            _LoginHistoryRepo = LoginHistoryRepo;
        }
        public async Task<PaginatedList<AuditLogDTO>> Handle(GetAllLogHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _LoginHistoryRepo.GetAll()
                            .PaginateAsync<AuditLog, AuditLogDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }

    }
}




