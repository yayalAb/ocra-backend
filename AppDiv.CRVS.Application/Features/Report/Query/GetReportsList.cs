
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Report.Query
{
    // Customer query with List<Customer> response
    public record GetReportsList : IRequest<PaginatedList<ReportStoreDTO>>
    {
        public int? PageCount { get; set; } = 1;
        public int? PageSize { get; set; } = 10;

    }

    public class GetReportsListHandler : IRequestHandler<GetReportsList, PaginatedList<ReportStoreDTO>>
    {
        private readonly IReportStoreRepostory _reportRepository;

        public GetReportsListHandler(IReportStoreRepostory reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<PaginatedList<ReportStoreDTO>> Handle(GetReportsList request, CancellationToken cancellationToken)
        {
            var Report = _reportRepository.GetAll()
                                .Select(repo => new ReportStoreDTO
                                            {
                                                Id = repo.Id,
                                                ReportName = repo.ReportName,
                                                ReportTitle =repo.ReportTitle,
                                                Groups=repo.UserGroups
                                            }).ToList();

            return await PaginatedList<ReportStoreDTO>
                            .CreateAsync(
                                 Report
                                , request.PageCount ?? 1, request.PageSize ?? 10);
        }
    }
}