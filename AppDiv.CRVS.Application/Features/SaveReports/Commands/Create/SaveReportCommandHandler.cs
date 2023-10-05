
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using ApplicationException = AppDiv.CRVS.Application.Exceptions.ApplicationException;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Interfaces;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Application.Features.SaveReports.Commands
{

    public class SaveReportCommandHandler : IRequestHandler<SaveReportCommand, SaveReportCommandResponse>
    {
        private readonly IMyReportRepository _ReportRepository;
        private readonly IUserResolverService _UserResolver;
        private readonly IUserRepository _userReportory;

        public SaveReportCommandHandler(IMyReportRepository ReportRepository, IUserResolverService UserResolver, IUserRepository userReportory)
        {
            _ReportRepository = ReportRepository;
            _UserResolver = UserResolver;
            _userReportory = userReportory;
        }
        public async Task<SaveReportCommandResponse> Handle(SaveReportCommand request, CancellationToken cancellationToken)
        {
            var SaveReportCommandResponse = new SaveReportCommandResponse();
            var validator = new SaveReportCommandValidetor(_ReportRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                SaveReportCommandResponse.Success = false;
                SaveReportCommandResponse.ValidationErrors = new List<string>();
                foreach (var error in validationResult.Errors)
                    SaveReportCommandResponse.ValidationErrors.Add(error.ErrorMessage);
                SaveReportCommandResponse.Message = SaveReportCommandResponse.ValidationErrors[0];
            }
            if (SaveReportCommandResponse.Success)
            {
                var user = _userReportory.GetAll().Where(x => x.PersonalInfoId == _UserResolver.GetUserPersonalId()).FirstOrDefault();


                if (user == null)
                {
                    throw new NotFoundException("User Does Not Found");
                }
                var Report = new MyReports
                {
                    ReportOwnerId = new Guid(user.Id),
                    ReportName = request.ReportName,
                    ReportTitle=request.ReportTitle,
                    Description=request.Description,
                    Agrgate = JsonConvert.SerializeObject(request.Agrgate).ToString(),
                    Filter = request.Filter,
                    Colums = (request?.Colums == null || request?.Colums.Length == 0) ? "" : string.Join(",", request.Colums),
                    Other = request?.Other?.ToString(),
                    IsShared = false,
                    SharedFrom = Guid.Empty,
                    ReportGroupId=request?.ReportGroupId
                    
                };
                await _ReportRepository.InsertAsync(Report, cancellationToken);
                await _ReportRepository.SaveChangesAsync(cancellationToken);
                SaveReportCommandResponse.Message = "Report created successfully";

            }
            return SaveReportCommandResponse;
        }
    }
}
