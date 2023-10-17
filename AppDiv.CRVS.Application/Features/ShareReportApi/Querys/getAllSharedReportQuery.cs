using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace AppDiv.CRVS.Application.Features.ShareReportApi.Querys
{
    // Customer query with List<Customer> response
    public record getAllSharedReportQuery : IRequest<PaginatedList<SharedReportResponseDTO>>
    {
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
    }

    public class getAllSharedReportQueryHandler : IRequestHandler<getAllSharedReportQuery, PaginatedList<SharedReportResponseDTO>>
    {
        private readonly ISharedReportRepository _sharedReportRepository;
        public getAllSharedReportQueryHandler(ISharedReportRepository sharedReportQueryRepository)
        {
            _sharedReportRepository = sharedReportQueryRepository;
        }
        public async Task<PaginatedList<SharedReportResponseDTO>> Handle(getAllSharedReportQuery request, CancellationToken cancellationToken)
        {
            var ranges = _sharedReportRepository.GetAll();
            return await ranges.Select(re => new SharedReportResponseDTO
                {
                    Id =re.Id,
                    Username =re.Username,
                    UserRole =re.UserRole,
                    ReportTitle =re.ReportTitle
                })
            .PaginateAsync<SharedReportResponseDTO, SharedReportResponseDTO>(request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}