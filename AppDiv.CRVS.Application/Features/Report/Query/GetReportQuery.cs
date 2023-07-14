using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
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
        public int? PageCount { get; set; }
        public int? PageSize { get; set; }
    }

    public class GetReportQueryHandler : IRequestHandler<GetReportQuery, object>
    {
        private readonly IReportRepostory _reportRepository;

        public GetReportQueryHandler(IReportRepostory reportRepository)
        {
            _reportRepository = reportRepository;
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
            var Report = await _reportRepository.GetReportData(request.reportName, request.columns, request.filterse, request.aggregates);

            return PaginatedList<object>
                            .CreateAsync(
                                 Report
                                , request.PageCount ?? 1, request.PageSize ?? 10);

        }
    }
}