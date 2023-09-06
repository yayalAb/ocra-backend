using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Features.AuditLogs.Query;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using AppDiv.CRVS.Utility.Services;
using MediatR;
namespace AppDiv.CRVS.Application.Features.AuditLogs.Query
{
    public class EventAuditLogQueryHandler : IRequestHandler<EventAuditLogQuery, string>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICertificateRepository _certificateRepository;

        public EventAuditLogQueryHandler(IAuditLogRepository auditLogRepository, IUserRepository userRepository, ICertificateRepository certificateRepository)
        {
            this._certificateRepository = certificateRepository;
            this._userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<string> Handle(EventAuditLogQuery request, CancellationToken cancellationToken)
        {
            var certificate = _certificateRepository.GetAll();
            return "Comming soon!";
            
        }
    }
}
