using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Report.Query
{
    // Customer query with List<Customer> response
    public record GetReportQuery : IRequest<object>
    {
        public string reportName { get; set; }
        public List<string>? columns { get; set; }
        public string? filterse { get; set; }
        public List<Aggregate>? aggregates { get; set; }
        public int? PageCount { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }

    public class GetReportQueryHandler : IRequestHandler<GetReportQuery, object>
    {
        private readonly IReportRepostory _reportRepository;
        private readonly IReportStoreRepostory _reportStore;


        public GetReportQueryHandler(IReportRepostory reportRepository,IReportStoreRepostory reportStore)
        {
            _reportRepository = reportRepository;
            _reportStore = reportStore;
        }
        public async Task<object> Handle(GetReportQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.reportName))
            {
                return new BaseResponse
                {
                    Message = "Report Name must not be Empty",
                    Success = false
                };
            }
            var reportStore=_reportStore.GetAll().Where(x=>x.ReportName==request.reportName).FirstOrDefault();
            // ,
            var Report = await _reportRepository.GetReportData(request.reportName, request.columns, request.filterse, request.aggregates, (bool)reportStore.isAddressBased);

            var report=PaginatedList<object>
                            .CreateAsync(
                                 Report
                                , request.PageCount ?? 1, request.PageSize ?? 10);
            return new{
               reportStore.ReportTitle, 
               reportStore.ReportName,
               reportStore.columnsLang,
               reportStore.Other,
               report
            };

        }
    }
}