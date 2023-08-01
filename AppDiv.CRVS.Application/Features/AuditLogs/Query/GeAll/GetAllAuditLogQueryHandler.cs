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
    public class GetAllAuditLogQueryHandler : IRequestHandler<GetAllAuditLogQuery, PaginatedList<AuditGridDTO>>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserRepository _userRepository;

        public GetAllAuditLogQueryHandler(IAuditLogRepository auditLogRepository, IUserRepository userRepository)
        {
            this._userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<PaginatedList<AuditGridDTO>> Handle(GetAllAuditLogQuery request, CancellationToken cancellationToken)
        {
            var audit = _auditLogRepository.GetAll();
            if (request.Id != null)
            {
                audit = audit.Where(a => request.Id == a.TablePk);
            }
            if (request.AddressId != null)
            {
                audit = audit.Where(a => request.AddressId == a.AddressId);
            }
            if (request.UserId != null)
            {
                audit = audit.Where(a => request.UserId == a.AuditUserId);
            }
            if (request.StartDate != null && request.EndDate != null)
            {
                var convertor = new CustomDateConverter();
                var startDate = convertor.EthiopicToGregorian(request.StartDate);
                var endDate = convertor.EthiopicToGregorian(request.EndDate);
                audit = audit.Where(a => a.AuditDate >= startDate && a.AuditDate <= endDate);
            }

            if (request.EntityType != null)
            {
                audit = audit.Where(a => a.EntityType == request.EntityType);
            }
            return await audit.OrderByDescending(a => a.AuditDate).Select(a => new AuditGridDTO(a, request.WithContent, _userRepository))
                        .PaginateAsync<AuditGridDTO,AuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
