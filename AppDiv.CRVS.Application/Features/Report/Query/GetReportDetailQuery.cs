using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Report.Query
{
    // Customer query with List<Customer> response
    public record GetReportDetailQuery : IRequest<object>
    {
        public string? ReportName { get; set; }

    }

    public class GetReportDetailQueryHandler : IRequestHandler<GetReportDetailQuery, object>
    {
        private readonly IReportStoreRepostory _reportRepository;

        public GetReportDetailQueryHandler(IReportStoreRepostory reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<object> Handle(GetReportDetailQuery request, CancellationToken cancellationToken)
        {
            var Report = _reportRepository.GetAll().Where(x => x.ReportName == request.ReportName);

            return Report;
        }
    }
}