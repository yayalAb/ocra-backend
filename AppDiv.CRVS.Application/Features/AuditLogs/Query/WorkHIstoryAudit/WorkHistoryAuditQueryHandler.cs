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
    public class WorkHistoryAuditQueryHandler : IRequestHandler<WorkHistoryAuditQuery, PaginatedList<WorkHistoryAuditGridDTO>>
    {
        private readonly IWorkHistoryRepository _workHistoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogService _auditService;
        private readonly IEventRepository _eventRepository;
        private readonly IDateAndAddressService _addressService;

        public WorkHistoryAuditQueryHandler(IWorkHistoryRepository workHistoryRepository, IDateAndAddressService addressService)
        {
            this._addressService = addressService;
            this._workHistoryRepository = workHistoryRepository;
        }
        public async Task<PaginatedList<WorkHistoryAuditGridDTO>> Handle(WorkHistoryAuditQuery request, CancellationToken cancellationToken)
        {
            var history = _workHistoryRepository.GetAllGrid();
            
            return await history.OrderByDescending(a => a.CreatedAt).Select(h => new WorkHistoryAuditGridDTO(h))
                        .PaginateAsync<WorkHistoryAuditGridDTO,WorkHistoryAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
