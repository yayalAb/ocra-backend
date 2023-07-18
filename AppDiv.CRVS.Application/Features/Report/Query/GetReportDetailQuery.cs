using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Report.Query
{
    // Customer query with List<Customer> response
    public record GetReportDetailQuery : IRequest<ReportStore>
    {
        public Guid? Id { get; set; }

    }

    public class GetReportDetailQueryHandler : IRequestHandler<GetReportDetailQuery, ReportStore>
    {
        private readonly IReportStoreRepostory _reportRepository;

        public GetReportDetailQueryHandler(IReportStoreRepostory reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<ReportStore> Handle(GetReportDetailQuery request, CancellationToken cancellationToken)
        {
            var Report = await _reportRepository.GetAsync(request.Id);

            return Report;
        }
    }
}