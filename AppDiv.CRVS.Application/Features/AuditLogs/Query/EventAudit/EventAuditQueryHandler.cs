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
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class EventAuditQueryHandler : IRequestHandler<EventAuditQuery, PaginatedList<EventAuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IEventRepository _eventRepository;

        public EventAuditQueryHandler(IAuditLogRepository auditLogRepository, IAuditLogService auditService, IEventRepository eventRepository)
        {
            this._eventRepository = eventRepository;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<EventAuditGridDTO>> Handle(EventAuditQuery request, CancellationToken cancellationToken)
        {
            var events = new List<string> { "BirthEvent", "DeathEvent", "MarriageEvent", "DivorceEvent", "AdoptionEvent" };
            var audit = _auditLogRepository.GetAllGrid().Where(a => events.Contains(a.EntityType));
            if (request.SearchString is not null)
            {
                audit = audit.Where(a => EF.Functions.Like(a.AuditUser.UserName, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.EntityType, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Action!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.AuditData, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address.AddressNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address.ParentAddress.AddressNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address.ParentAddress.ParentAddress.AddressNameStr!, "%" + request.SearchString + "%")
                                    );
            }
            
            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new EventAuditGridDTO(a, _eventRepository))
                        .PaginateAsync<EventAuditGridDTO,EventAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
