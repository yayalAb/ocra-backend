
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
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
    public record GetReportsList : IRequest<object>
    {

    }

    public class GetReportsListHandler : IRequestHandler<GetReportsList, object>
    {
        private readonly IReportRepostory _reportRepository;

        public GetReportsListHandler(IReportRepostory reportRepository)
        {
            _reportRepository = reportRepository;
        }
        public async Task<object> Handle(GetReportsList request, CancellationToken cancellationToken)
        {
            var Report = await _reportRepository.GetReports();

            return Report;
        }
    }
}