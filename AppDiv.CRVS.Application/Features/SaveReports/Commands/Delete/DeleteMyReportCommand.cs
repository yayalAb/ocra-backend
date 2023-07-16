
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using MediatR;


namespace AppDiv.CRVS.Application.Features.SaveReports.Commands.Delete
{
    // Customer create command with BaseResponse response
    public class DeleteMyReportCommand : IRequest<BaseResponse>
    {
        public Guid Id { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class DeleteMyReportCommandHandler : IRequestHandler<DeleteMyReportCommand, BaseResponse>
    {
        private readonly IMyReportRepository _myReportRepository;
        public DeleteMyReportCommandHandler(IMyReportRepository myReportRepository)
        {
            _myReportRepository = myReportRepository;
        }

        public async Task<BaseResponse> Handle(DeleteMyReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Id == null || request.Id == Guid.Empty)
                {
                    throw new NotFoundException("Report With the given Id Does not Found");

                }
                var report = await _myReportRepository.GetAsync(request.Id);
                if (report == null)
                {
                    throw new NotFoundException("Report With the given Id Does not Found");
                }
                await _myReportRepository.DeleteAsync(request.Id);
                await _myReportRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exp)
            {
                throw (new NotFoundException(exp.Message));
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