using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;


namespace AppDiv.CRVS.Application.Features.ShareReportApi.commands.delete
{
    // Customer create command with BaseResponse response
    public class DeleteReportApi : IRequest<BaseResponse>
    {
        public Guid[] Ids { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteReportApiHandler : IRequestHandler<DeleteReportApi, BaseResponse>
    {
        private readonly ISharedReportRepository _sharedReportIpiRepository;
        public DeleteReportApiHandler(ISharedReportRepository sharedReportIpiRepository)
        {
            _sharedReportIpiRepository = sharedReportIpiRepository;
        }

        public async Task<BaseResponse> Handle(DeleteReportApi request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            try
            {
                IEnumerable<Guid> ids = request.Ids;
                foreach (Guid x in ids)
                {
                    await _sharedReportIpiRepository.DeleteAsync(x); ;
                }
                await _sharedReportIpiRepository.SaveChangesAsync(cancellationToken);

                response.Deleted("Report Api");
            }
            catch (Exception e)
            {
                response.BadRequest("Cannot delete the specified Report API because it is being used.");
            }

            return response;
        }
    }
}