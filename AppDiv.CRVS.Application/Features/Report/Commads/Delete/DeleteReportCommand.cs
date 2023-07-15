using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Report.Commads.Delete
{
    // Customer create command with BaseResponse response
    public class DeleteReportCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteReportCommandHandler : IRequestHandler<DeleteReportCommand, BaseResponse>
    {
        private readonly IReportRepostory _reportRepository;
        private readonly IReportStoreRepostory _reportStoreRepository;
        public DeleteReportCommandHandler(IReportRepostory reportRepository, IReportStoreRepostory reportStoreRepository)
        {
            _reportRepository = reportRepository;
            _reportStoreRepository = reportStoreRepository;
        }

        public async Task<BaseResponse> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var SelectedReport = await _reportStoreRepository.GetAsync(request.Id);
                if (SelectedReport != null)
                {
                    await _reportRepository.DeleteReport(SelectedReport?.ReportName);
                }

                await _reportStoreRepository.DeleteAsync(request.Id);
                await _reportStoreRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw (new ApplicationException(exp.Message));
            }

            var res = new BaseResponse
            {
                Success = true,
                Message = "Report information has been deleted!"
            };
            return res;
        }
    }
}