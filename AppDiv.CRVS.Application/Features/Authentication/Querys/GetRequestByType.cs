using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Features.Authentication.Querys
{
    public class GetRequestByType : IRequest<PaginatedList<AuthenticationRequestListDTO>>
    {
        public string RequestType { get; set; } = "change";
        public string Status { get; set; }
        public bool IsYourRequestList { get; set; } = false;
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }
        public string? startDate { get; set; }
        public string? endDate { get; set; }

    }
    public class GetRequestByTypeHandler : IRequestHandler<GetRequestByType, PaginatedList<AuthenticationRequestListDTO>>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IWorkflowRepository _WorkflowRepo;
        private readonly IRequestRepostory _RequestRepostory;
        private readonly IUserRepository _UserRepo;
        private readonly IUserResolverService _ResolverService;
        private readonly IDateAndAddressService _dateAndAddressService;
        private readonly ITransactionService _transactionService;


        public GetRequestByTypeHandler(ITransactionService transactionService, IDateAndAddressService dateAndAddressService, IUserRepository UserRepo, IUserResolverService ResolverService, IWorkflowRepository WorkflowRepo, IRequestRepostory RequestRepostory, IWorkflowService WorkflowService)
        {
            _WorkflowService = WorkflowService;
            _RequestRepostory = RequestRepostory;
            _WorkflowRepo = WorkflowRepo;
            _UserRepo = UserRepo;
            _ResolverService = ResolverService;
            _dateAndAddressService=dateAndAddressService;
            _transactionService = transactionService;
        }
        public async Task<PaginatedList<AuthenticationRequestListDTO>> Handle(GetRequestByType request, CancellationToken cancellationToken)
        {
              var RequestList = _transactionService.GetAll()
                  .Include(x=>x.CivilRegOfficer)
                  .ThenInclude(x=>x.UserGroups)
                  .Include(r=>r.Request)
                  .Include(x=>x.Workflow)
                  .ThenInclude(x=>x.Steps).Where(x=>x.Request.isDeleted==false)
                 .AsQueryable();
            if(!string.IsNullOrEmpty(request.startDate)&&!string.IsNullOrEmpty(request.endDate)){
                var converter=new CustomDateConverter();
                DateTime startDate=converter.EthiopicToGregorian(request.startDate);
                DateTime endDate=converter.EthiopicToGregorian(request.endDate);
                RequestList=RequestList.Where(x=>x.CreatedAt>=startDate && x.CreatedAt<= endDate );
            }
            else{
                DateTime lastMonth=DateTime.Now.AddDays(-30);
                RequestList=RequestList.Where(x=>x.CreatedAt >= lastMonth);
            }
            RequestList = request.RequestType switch
            {
                "change" => RequestList.Include(x => x.Request.CorrectionRequest.Event.EventOwener),
                "authentication" => RequestList.Include(x => x.Request.AuthenticationRequest.Certificate.Event.EventOwener),
                "verification" => RequestList.Include(x => x.Request.VerficationRequest.Event.EventOwener),
                _ => RequestList // Default case when the request type doesn't match any of the above conditions
            };
            RequestList = request.Status switch
            {
                "approved" => RequestList.Where(r => r.Request.IsRejected == false && (request.IsYourRequestList ? r.Request.currentStep == r.Request.NextStep : r.ApprovalStatus == true)),
                "rejected" => RequestList.Where(r => r.ApprovalStatus == false && r.Request.IsRejected == true),
                "rejectedOnce" => RequestList.Where(r => r.ApprovalStatus == false && r.Request.IsRejected == true && r.CurrentStep == 0),
                "inprogress" => RequestList.Where(r => r.Request.currentStep != r.Request.NextStep && r.Request.IsRejected == false),
                _ => RequestList // Default case when the status doesn't match any of the above conditions
            };
            if (request.IsYourRequestList)
            {
                RequestList = RequestList
                    .Where(r => r.Request.CivilRegOfficerId != r.CivilRegOfficer.PersonalInfoId)
                    .Where(c => _transactionService.GetAll()
                        .Where(t => t.RequestId == c.RequestId && t.CreatedAt > c.CreatedAt)
                        .Count() == 0);
            }
                 
            
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                RequestList = RequestList.Where(
                    u => EF.Functions.Like(u.CivilRegOfficer.PersonalInfo.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CivilRegOfficer.PersonalInfo.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CivilRegOfficer.PersonalInfo.LastNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Request.RequestType!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Request.currentStep.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CreatedAt.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.Request.NextStep.ToString()!, "%" + request.SearchString + "%"));
            }

            var RequestListDto = RequestList.Where(x =>  x.Request.RequestType == request.RequestType && 
                  (request.IsYourRequestList ? x.Request.CivilRegOfficerId == _ResolverService.GetUserPersonalId() : x.CivilRegOfficerId == _ResolverService.GetUserId()))
             .OrderByDescending(w => w.CreatedAt)
             .Select(t => new AuthenticationRequestListDTO
             {
                Id = t.Request.Id,
                TransactionId = t.Id,
                OfficerId = Guid.Parse(t.CivilRegOfficerId),
                RequestedBy = t.Request.CivilRegOfficer.FullNameLang,
                RequestType = t.Request.RequestType,
                EventId = (t.Request.AuthenticationRequest != null)  ? t.Request.AuthenticationRequest.Certificate.EventId :
                            t.Request.CorrectionRequest != null ? t.Request.CorrectionRequest.EventId :
                            t.Request.VerficationRequest != null ? t.Request.VerficationRequest.EventId : null,
                RequestId = (t.Request.CorrectionRequest == null) ? (t.Request.AuthenticationRequest == null) ?
                    t.Request.PaymentExamptionRequest.Id : _WorkflowService.GetEventId(t.Request.AuthenticationRequest.CertificateId) : t.Request.CorrectionRequest.Id,
                EventType = (t.Request.AuthenticationRequest != null)  ? t.Request.AuthenticationRequest.Certificate.Event.EventType :
                            t.Request.CorrectionRequest != null ? t.Request.CorrectionRequest.Event.EventType :
                            t.Request.VerficationRequest != null ? t.Request.VerficationRequest.Event.EventType : string.Empty,
                CertificateId = (t.Request.AuthenticationRequest != null) ? t.Request.AuthenticationRequest.Certificate.Event.CertificateId :
                                t.Request.CorrectionRequest != null ? t.Request.CorrectionRequest.Event.CertificateId :
                            t.Request.VerficationRequest != null ? t.Request.VerficationRequest.Event.CertificateId : string.Empty,
                            
                OwnerFullName = (t.Request.AuthenticationRequest != null) ? t.Request.AuthenticationRequest.Certificate.Event.EventOwener.FullNameLang :
                                t.Request.CorrectionRequest != null ? t.Request.CorrectionRequest.Event.EventOwener.FullNameLang : 
                            t.Request.VerficationRequest != null ? t.Request.VerficationRequest.Event.EventOwener.FullNameLang : string.Empty,
                
                EventRegDate =(t.Request.AuthenticationRequest != null) ? t.Request.AuthenticationRequest.Certificate.Event.EventDateEt :
                                t.Request.CorrectionRequest != null ? t.Request.CorrectionRequest.Event.EventDateEt: 
                            t.Request.VerficationRequest != null ? t.Request.VerficationRequest.Event.EventDateEt : "",
                CurrentStep = t.Request.currentStep,
                NextStep = t.Request.NextStep,
                RequestDate =new CustomDateConverter(t.Request.CreatedAt).ethiopianDate,
                ActionBy=t.CivilRegOfficer.UserName,
                UserGroups=t.CivilRegOfficer.UserGroups.Select(x=>x.GroupName).FirstOrDefault(),
                ActionDate=new CustomDateConverter(t.CreatedAt).ethiopianDate,
                ResponsbleGroup=t.Request.Workflow.Steps.Where(s=>s.step==t.Request.NextStep).Select(x=>x.UserGroup.GroupName).SingleOrDefault()
             });
            var List = await PaginatedList<AuthenticationRequestListDTO>
                             .CreateAsync(
                                  RequestListDto
                                 , request.PageCount ?? 1, request.PageSize ?? 10);
            return List;
        }
    }
}
