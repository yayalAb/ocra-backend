using System.Text.Json;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Report.Query
{
    // Customer query with List<Customer> response
    public record GetReportDetailQuery : IRequest<ReportDetailResponsDTo>
    {
        public Guid? Id { get; set; }

    }

    public class GetReportDetailQueryHandler : IRequestHandler<GetReportDetailQuery, ReportDetailResponsDTo>
    {
        private readonly IReportStoreRepostory _reportRepository;

        public GetReportDetailQueryHandler(IReportStoreRepostory reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<ReportDetailResponsDTo> Handle(GetReportDetailQuery request, CancellationToken cancellationToken)
        {
            var Report = await _reportRepository.GetAsync(request.Id);
            var response=new ReportDetailResponsDTo{
                ReportName=Report.ReportName,
                ReportTitle=Report.ReportTitle,
                Description=Report.Description,
                DefualtColumns=Report.DefualtColumns,
                Query=Report.Query,
                ColumnsLang=JsonSerializer.Deserialize<List<ReportColumsLngDto>>(Report.columnsLang),

            };
            return response;
        }
    }
}