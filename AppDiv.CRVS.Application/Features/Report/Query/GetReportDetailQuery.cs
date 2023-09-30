using System.Text.Json;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using MediatR;
using Newtonsoft.Json.Linq;

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
            if(request.Id==null||request.Id==Guid.Empty){
                throw new NotFoundException("Report Id must not be Empty");
            }
            var Report = await _reportRepository.GetAsync(request.Id);

            if(Report==null){
               throw new NotFoundException("Report with the given Id does't Found"); 
            }
            var response=new ReportDetailResponsDTo{
                ReportName=Report.ReportName,
                ReportTitle=Report.ReportTitle,
                Description=Report.Description,
                DefualtColumns=Report.DefualtColumns,
                Query=Report.Query,
                ColumnsLang=string.IsNullOrEmpty(Report.columnsLang)? null : JsonSerializer.Deserialize<List<ReportColumsLngDto>>(Report.columnsLang),
                UserGroups=Report.UserGroups,
                isAddressBased=Report.isAddressBased,
                Other=string.IsNullOrEmpty(Report.Other)? null : JObject.Parse(Report.Other)
             };
            return response;
        }
    }
}