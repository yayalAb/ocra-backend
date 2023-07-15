using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;


namespace AppDiv.CRVS.Application.Features.SaveReports.Commands.Share
{
    // Customer create command with BaseResponse response
    public class ShareMyReportCommands : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }
        public Guid[] UsersId { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class ShareMyReportCommandsHandler : IRequestHandler<ShareMyReportCommands, BaseResponse>
    {
        private readonly IMyReportRepository _myReportRepository;
        public ShareMyReportCommandsHandler(IMyReportRepository myReportRepository)
        {
            _myReportRepository = myReportRepository;
        }

        public async Task<BaseResponse> Handle(ShareMyReportCommands request, CancellationToken cancellationToken)
        {
            try
            {
                var mayreport = await _myReportRepository.GetAsync(request.Id);
                if (mayreport != null)
                {
                    foreach (var user in request.UsersId)
                    {
                        var Report = new MyReports
                        {
                            ReportOwnerId = user,
                            ReportName = mayreport?.ReportName,
                            Agrgate = mayreport?.Agrgate,
                            Filter = mayreport?.Filter,
                            Colums = mayreport?.Colums,
                            Other = mayreport?.Other,
                            IsShared = true,
                            SharedFrom = mayreport?.ReportOwnerId,
                        };
                        await _myReportRepository.InsertAsync(Report, cancellationToken);
                    }
                }
                await _myReportRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            var res = new BaseResponse
            {
                Success = true,
                Message = "My Report information has been deleted!"
            };
            return res;
        }
    }
}