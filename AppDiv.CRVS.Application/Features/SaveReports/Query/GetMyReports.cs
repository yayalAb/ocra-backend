using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
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

namespace AppDiv.CRVS.Application.Features.SaveReports.Query
{
    // Customer query with List<Customer> response
    public record GetMyReports : IRequest<List<MyReports>>
    {

    }

    public class GetMyReportsHandler : IRequestHandler<GetMyReports, List<MyReports>>
    {
        private readonly IMyReportRepository _reportRepository;
        private readonly IUserResolverService _UserResolver;
        private readonly IUserRepository _userReportory;


        public GetMyReportsHandler(IMyReportRepository reportRepository, IUserResolverService UserResolver, IUserRepository userReportory)
        {
            _reportRepository = reportRepository;
            _UserResolver = UserResolver;
            _userReportory = userReportory;
        }
        public async Task<List<MyReports>> Handle(GetMyReports request, CancellationToken cancellationToken)
        {
            var user = _userReportory.GetAll().Where(x => x.PersonalInfoId == _UserResolver.GetUserPersonalId()).FirstOrDefault();


            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            var reports = _reportRepository.GetAll().Where(x => x.ReportOwnerId.ToString() == user.Id);

            return reports.ToList();

        }
    }
}