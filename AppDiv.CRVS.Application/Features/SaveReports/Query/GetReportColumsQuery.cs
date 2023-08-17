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
    public record GetReportColumsQuery : IRequest<object>
    {
        public string ViewName { get; set; }

    }

    public class GetReportColumsQueryHandler : IRequestHandler<GetReportColumsQuery, object>
    {
        private readonly IReportRepostory _reportRepository;
        private readonly IUserResolverService _UserResolver;
        private readonly IUserRepository _userReportory;


        public GetReportColumsQueryHandler(IReportRepostory reportRepository, IUserResolverService UserResolver, IUserRepository userReportory)
        {
            _reportRepository = reportRepository;
            _UserResolver = UserResolver;
            _userReportory = userReportory;
        }
        public async Task<object> Handle(GetReportColumsQuery request, CancellationToken cancellationToken)
        {
            var user = _userReportory.GetAll().Where(x => x.PersonalInfoId == _UserResolver.GetUserPersonalId()).FirstOrDefault();

            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            var reports = _reportRepository.GetReportColums(request.ViewName);

            return reports.Result;

        }
    }
}