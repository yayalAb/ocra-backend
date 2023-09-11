using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class SystemAuditQueryHandler : IRequestHandler<SystemAuditQuery, PaginatedList<SystemAuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public SystemAuditQueryHandler(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<SystemAuditGridDTO>> Handle(SystemAuditQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAllGrid().Where(a => a.EntityType == "Lookup" || a.EntityType == "Address" || a.EntityType == "Setting");
            if (request.SearchString is not null)
            {
                audit = audit.Where(a => EF.Functions.Like(a.AuditUser.UserName, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.AuditData, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.EntityType, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Action, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address!.AddressNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address!.ParentAddress!.AddressNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address!.ParentAddress!.ParentAddress!.AddressNameStr!, "%" + request.SearchString + "%")
                                    );
            }
            
            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new SystemAuditGridDTO(a))
                        .PaginateAsync<SystemAuditGridDTO,SystemAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
