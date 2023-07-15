using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;

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
                await _ReportRepository.CreateReportAsync(request.ReportName, request.Query, request?.Description, request?.DefualtColumns, request?.ReportTitle, cancellationToken);
                CreateReportCommadResponse.Message = "Report created successfully";

            }
            return CreateReportCommadResponse;
        }
    }
}
