using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;


namespace AppDiv.CRVS.Application.Features.Report.Commads.Delete
{
    // Customer create command with BaseResponse response
    public class DeleteReportCommand : IRequest<BaseResponse>
    {
        public Guid[] Ids { get; set; }

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
                if(request.Ids==null||request.Ids.Length==0){
                        throw new NotFoundException("Report Id must not be Empty");
                    }
                foreach(Guid Id in request.Ids){
                var SelectedReport = await _reportStoreRepository.GetAsync(Id);

                if (SelectedReport != null)
                {
                    await _reportRepository.DeleteReport(SelectedReport?.ReportName);
                }
                else{
                    throw new NotFoundException("Report with the given Id does't Found"); 
                }

                await _reportStoreRepository.DeleteAsync(Id);
                await _reportStoreRepository.SaveChangesAsync(cancellationToken);

                }    

            }
            catch (Exception exp)
            {
                throw new NotFoundException(exp.Message);
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