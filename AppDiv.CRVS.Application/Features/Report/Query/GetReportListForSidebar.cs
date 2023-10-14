using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Report.Query
{
    // Customer query with List<Customer> response
    public record GetReportListForSidebar : IRequest<object>
    {

    }

    public class GetReportListForSidebarHandler : IRequestHandler<GetReportListForSidebar, object>
    {
        private readonly IReportStoreRepostory _reportRepository;
        private readonly IMyReportRepository _myReportRepository;
        private readonly IUserResolverService _userResolverService;
        private readonly IUserRepository _userRepository;
        public GetReportListForSidebarHandler(IUserRepository userRepository, IUserResolverService userResolverService,IReportStoreRepostory reportRepository, IMyReportRepository myreportRepostory)
        {
            _reportRepository = reportRepository;
            _myReportRepository = myreportRepostory;
            _userResolverService=userResolverService;
            _userRepository=userRepository;
        }
        public async Task<object> Handle(GetReportListForSidebar request, CancellationToken cancellationToken)
        {   var userData= _userRepository.GetAll()
                .Include(x=>x.UserGroups)
                .Where(x=>x.Id==_userResolverService.GetUserId()).FirstOrDefault();
             List<Guid> GroupIds = userData.UserGroups.Select(g => g.Id).ToList();
             var MyReport = _myReportRepository.GetAll()
            .Include(x=>x.ReportGroup)
            .Where(x => x.ReportOwnerId.ToString() == userData.Id)
            .Select(repo => new ReportStoreDTO
            {
                Id = repo.Id,
                ReportName = repo.ReportName,
                ReportTitle = repo.ReportTitleLang,
                ReportGroup=repo.ReportGroup.ValueLang
            }).ToList();

             var Report = _reportRepository.GetAll()
                                .Include(x=>x.ReportGroup)
                                .Select(repo => new ReportStoreDTO
                                {
                                    Id = repo.Id,
                                    ReportName = repo.ReportName,
                                    ReportTitle = repo.ReportTitleLang,
                                    Groups = JsonConvert.DeserializeObject<List<Guid>>(repo.UserGroupsStr),
                                    ReportGroup=repo.ReportGroup.ValueLang
                                })
                                .AsEnumerable()
                                .Where(report => report.Groups != null && GroupIds != null && report.Groups.Intersect(GroupIds).Any())
                                .ToList();
            return new {
                    Report,
                    MyReport
                };
        }
    }
}