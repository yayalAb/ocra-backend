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
    public class WorkHistoryAuditQueryHandler : IRequestHandler<WorkHistoryAuditQuery, PaginatedList<WorkHistoryAuditGridDTO>>
    {
        private readonly IWorkHistoryRepository _workHistoryRepository;

        public WorkHistoryAuditQueryHandler(IWorkHistoryRepository workHistoryRepository)
        {
            this._workHistoryRepository = workHistoryRepository;
        }
        public async Task<PaginatedList<WorkHistoryAuditGridDTO>> Handle(WorkHistoryAuditQuery request, CancellationToken cancellationToken)
        {
            var history = _workHistoryRepository.GetAllGrid();
            if (request.AddressId != null)
            {
                history = history.Where(a => request.AddressId == a.AddressId);
            }
            if (request.UserId != null)
            {
                history = history.Where(a => request.UserId == a.UserId);
            }
            if (request.StartDate != null && request.EndDate != null)
            {
                var convertor = new CustomDateConverter();
                var startDate = convertor.EthiopicToGregorian(request.StartDate);
                var endDate = convertor.EthiopicToGregorian(request.EndDate);
                history = history.Where(a => a.StartDate >= startDate && endDate <= a.CreatedAt);
            }
            if (request.SearchString is not null)
            {
                history = history.Where(a => EF.Functions.Like(a.User.UserName, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address!.AddressNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.User.PersonalInfo.FirstNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.User.PersonalInfo.MiddleNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.User.PersonalInfo.LastNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address!.ParentAddress!.AddressNameStr!, "%" + request.SearchString + "%")
                                      || EF.Functions.Like(a.Address!.ParentAddress!.ParentAddress!.AddressNameStr!, "%" + request.SearchString + "%")
                                    );
            }
            
            return await history.OrderByDescending(a => a.CreatedAt).Select(h => new WorkHistoryAuditGridDTO(h))
                        .PaginateAsync<WorkHistoryAuditGridDTO,WorkHistoryAuditGridDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}
