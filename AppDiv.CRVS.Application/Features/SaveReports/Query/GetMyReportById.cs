
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Enums;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.SaveReports.Query
{
    // Customer query with List<Customer> response
    public record GetMyReportById : IRequest<object>
    {
        public Guid Id { get; set; }
        public int? PageCount { get; set; } = 1;
        public int? PageSize { get; set; } = 10;

    }

    public class GetMyReportByIdHandler : IRequestHandler<GetMyReportById, object>
    {
        private readonly IMyReportRepository _reportRepository;
        private readonly IReportRepostory _reportRepo;
        private readonly IUserResolverService _UserResolver;
        private readonly IUserRepository _userReportory;


        public GetMyReportByIdHandler(IReportRepostory reportRepo, IMyReportRepository reportRepository, IUserResolverService UserResolver, IUserRepository userReportory)
        {
            _reportRepository = reportRepository;
            _UserResolver = UserResolver;
            _userReportory = userReportory;
            _reportRepo = reportRepo;
        }
        public async Task<object> Handle(GetMyReportById request, CancellationToken cancellationToken)
        {
            var user = _userReportory.GetAll().Where(x => x.PersonalInfoId == _UserResolver.GetUserPersonalId()).FirstOrDefault();
            if (user == null)
            {
                throw new NotFoundException("User Not Found");
            }
            var SavedReport = await _reportRepository.GetAsync(request.Id);
            List<string>? columns = string.IsNullOrEmpty(SavedReport?.Colums) ? null : SavedReport?.Colums?.Split(',').ToList();
            string? filterse = SavedReport?.Filter;
            List<Aggregate>? aggregates = new List<Aggregate>();
            if (SavedReport?.Agrgate != null && SavedReport?.Agrgate.Length > 2)
            {
                JArray jsonArray = JArray.Parse(SavedReport?.Agrgate);
                string json = JsonConvert.SerializeObject(SavedReport?.Agrgate);
                Console.WriteLine("Jeson arra : {0}", jsonArray);
                foreach (var jos in jsonArray)
                {
                    int aggregateMethodValue = (int)jos["AggregateMethod"];
                    SqlAggregate aggregateMethod = (SqlAggregate)Enum.Parse(typeof(SqlAggregate), aggregateMethodValue.ToString());
                    var agrgate = new Aggregate
                    {
                        PropertyName = (string)jos["PropertyName"],
                        AggregateMethod = aggregateMethod,
                    };
                    aggregates.Add(agrgate);
                }
            }
            else
            {
                aggregates = null;
            }
            Console.WriteLine("aggragate : {0}", aggregates.ToList());

            var Report = await _reportRepo.GetReportData(SavedReport.ReportName, columns, filterse, aggregates);

            return await PaginatedList<object>
                            .CreateAsync(
                                 Report
                                , request.PageCount ?? 1, request.PageSize ?? 10);


        }
    }
}