using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;
namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class LoginAuditQueryHandler : IRequestHandler<LoginAuditQuery, PaginatedList<LoginAuditGridDTO>>
    {
        private readonly ILoginHistoryRepository _loginHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditService;
        private readonly IEventRepository _eventRepository;

        public LoginAuditQueryHandler(ILoginHistoryRepository loginHistoryRepository, IUserRepository userRepository, IAuditLogService auditService, IEventRepository eventRepository)
        {
            this._userRepository = userRepository;
            this._auditService = auditService;
            this._eventRepository = eventRepository;
            _loginHistoryRepository = loginHistoryRepository;
        }
        public async Task<PaginatedList<LoginAuditGridDTO>> Handle(LoginAuditQuery request, CancellationToken cancellationToken)
        {
            var history = _loginHistoryRepository.GetAllGrid();
            
            return await history.OrderByDescending(l => l.EventDate).Select(a => new LoginAuditGridDTO(a))
                        .PaginateAsync<LoginAuditGridDTO,LoginAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
