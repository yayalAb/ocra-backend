using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Extensions;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace AppDiv.CRVS.Application.Features.ShareReportApi.Querys
{
    // Customer query with List<Customer> response
    public record getSharedReportById : IRequest<SharedReportDetailResponseDTO>
    {
        public Guid Id { get; set; }
        
    }

    public class getSharedReportByIdHandler : IRequestHandler<getSharedReportById, SharedReportDetailResponseDTO>
    {
        private readonly ISharedReportRepository _sharedReportRepository;
        public getSharedReportByIdHandler(ISharedReportRepository sharedReportQueryRepository)
        {
            _sharedReportRepository = sharedReportQueryRepository;
        }
        public async Task<SharedReportDetailResponseDTO> Handle(getSharedReportById request, CancellationToken cancellationToken)
        {
            var report = _sharedReportRepository.GetAll().Where(x=>x.Id==request.Id).FirstOrDefault();
            return new SharedReportDetailResponseDTO
                {
                    Id=report.Id,
                    ReportName=report.ReportName,
                    ReportTitle =report.ReportTitle,   
                    Agrgate =report.Agrgate,    
                    Filter =report.Filter,    
                    Colums =report.Colums,    
                    Other =report.Other,    
                    Username =report.Username,  
                    UserRole =report.UserRole,    
                    Email  = report.Email
                };
        }
    }
}