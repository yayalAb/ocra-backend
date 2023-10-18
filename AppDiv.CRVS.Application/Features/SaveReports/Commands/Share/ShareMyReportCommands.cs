using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using MediatR;


namespace AppDiv.CRVS.Application.Features.SaveReports.Commands.Share
{
    // Customer create command with BaseResponse response
    public class ShareMyReportCommands : IRequest<BaseResponse>
    {
        public string  ReportName { get; set; }
        public Guid[] UsersId { get; set; }

    }

    // Customer delete command handler with BaseResponse response as output
    public class ShareMyReportCommandsHandler : IRequestHandler<ShareMyReportCommands, BaseResponse>
    {
        private readonly IMyReportRepository _myReportRepository;
        private readonly IReportStoreRepostory _ReportStoreRepository;
        public ShareMyReportCommandsHandler(IMyReportRepository myReportRepository,IReportStoreRepostory ReportStoreRepository)
        {
            _myReportRepository = myReportRepository;
            _ReportStoreRepository=ReportStoreRepository;
        }

        public async Task<BaseResponse> Handle(ShareMyReportCommands request, CancellationToken cancellationToken)
        {
            try
            {
                var mayreport =  _myReportRepository.GetAll().Where(x=>x.ReportName==request.ReportName).FirstOrDefault();
                if(mayreport==null){
                 var  Report =  _ReportStoreRepository.GetAll().Where(x=>x.ReportName==request.ReportName).FirstOrDefault();
                 if(Report==null){
                    throw new NotFoundException("Report With the Given Id does not Found");
                 }
                  if (Report != null)
                   {
                    foreach (var user in request.UsersId)
                    {
                        var Report1 = new MyReports
                        {
                            ReportOwnerId = user,
                            ReportName = Report?.ReportName,
                            Colums = Report?.DefualtColumns,
                            Other = Report?.Other,
                            IsShared = true,
                            SharedFrom =Report?.CreatedBy,
                        };
                        await _myReportRepository.InsertAsync(Report1, cancellationToken);
                    }
                }
                }

                else 
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
                throw (new NotFoundException(exp.Message));
            }

            var res = new BaseResponse
            {
                Success = true,
                Message = "Report Shared!"
            };
            return res;
        }
    }
}