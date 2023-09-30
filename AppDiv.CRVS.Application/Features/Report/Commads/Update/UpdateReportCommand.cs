
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Features.Report.Commads.Update
{
    // Customer create command with CustomerResponse
    public class UpdateReportCommand : IRequest<ReportStore>
    {
        public Guid Id { get; set; }
        public string? ReportName { get; set; }
        public string? ReportTitle { get; set; }
        public string? Description { get; set; }
        public string[]? DefualtColumns { get; set; }
        public string? query { get; set; }
        public ReportColumsLngDto[] ColumnsLang { get; set; }
        public   List<Guid>? UserGroups { get; set; }
        public  bool? isAddressBased { get; set; }=false;
        public JObject? Other { get; set; }

    }

    public class UpdateReportCommandHandler : IRequestHandler<UpdateReportCommand, ReportStore>
    {
        private readonly IReportStoreRepostory _reportStoreRepository;
        private readonly IReportRepostory _reportRepository;
        public UpdateReportCommandHandler(IReportStoreRepostory reportStoreRepository, IReportRepostory reportRepository)
        {
            _reportStoreRepository = reportStoreRepository;
            _reportRepository = reportRepository;
        }
        public async Task<ReportStore> Handle(UpdateReportCommand request, CancellationToken cancellationToken)
        {
            // var customerEntity = CustomerMapper.Mapper.Map<Customer>(request);

            var reportStore = new ReportStore()
            {
                Id = request.Id,
                ReportName = request.ReportName,
                ReportTitle = request.ReportTitle,
                Description = request.Description,
                DefualtColumns = (request?.DefualtColumns == null || request?.DefualtColumns.Count() == 0) ? "" : string.Join(",", request?.DefualtColumns),
                columnsLang=JsonSerializer.Serialize(request.ColumnsLang),
                UserGroups=request.UserGroups,
                isAddressBased=request.isAddressBased,
                Other=request.Other.ToString()
            
            };
            try
            {
                if (!string.IsNullOrEmpty(request?.query) || !(request?.query.Count() < 15))
                {
                    await _reportRepository.UpdateReportQuery(request.ReportName, request.query);
                }
                await _reportStoreRepository.UpdateAsync(reportStore, x => x.Id);
                var result = await _reportStoreRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw new ApplicationException(exp.Message);
            }

            var modifiedreportStore = _reportStoreRepository.GetAll().Where(x => x.Id == request.Id).FirstOrDefault();
            var reportStoreResponse = CustomMapper.Mapper.Map<ReportStore>(modifiedreportStore);
            return reportStoreResponse;
        }
    }
}