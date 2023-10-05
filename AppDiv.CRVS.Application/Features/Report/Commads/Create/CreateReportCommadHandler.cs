using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using System.Text.Json;

namespace AppDiv.CRVS.Application.Features.Report.Commads
{

    public class CreateReportCommadHandler : IRequestHandler<CreateReportCommad, CreateReportCommadResponse>
    {
        private readonly IReportRepostory _ReportRepository;

        public CreateReportCommadHandler(IReportRepostory ReportRepository)
        {
            _ReportRepository = ReportRepository;
        }
        public async Task<CreateReportCommadResponse> Handle(CreateReportCommad request, CancellationToken cancellationToken)
        {
            var CreateReportCommadResponse = new CreateReportCommadResponse();
            var validator = new CreateReportCommadValidetor(_ReportRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                CreateReportCommadResponse.Success = false;
                CreateReportCommadResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    CreateReportCommadResponse.ValidationErrors.Add(error.ErrorMessage);
                CreateReportCommadResponse.Message = CreateReportCommadResponse.ValidationErrors[0];
            }
            if (CreateReportCommadResponse.Success)
            {
                string defualt = (request.DefualtColumns != null && request.DefualtColumns.Count() != 0) ? string.Join(",", request.DefualtColumns) : "";
                 var Report = new ReportStore
                    {
                        
                        Id = Guid.NewGuid(),
                        ReportName =request.ReportName,
                        ReportTitle =request.ReportTitle,
                        Description =request.Description,
                        DefualtColumns =defualt,
                        CreatedAt =DateTime.Now,
                        Query =request.Query,
                        columnsLang=JsonSerializer.Serialize(request.ColumnsLang),
                        UserGroups=request.UserGroups,
                        isAddressBased=request.isAddressBased,
                        Other=request.Other.ToString(),
                        ReportGroupId=request.ReportGroupId
                    };
                await _ReportRepository.CreateReportAsync(Report, cancellationToken);
                CreateReportCommadResponse.Message = "Report created successfully";
            }
            return CreateReportCommadResponse;
        }
    }
}
