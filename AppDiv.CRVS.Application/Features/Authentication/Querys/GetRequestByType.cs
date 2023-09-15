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
                 .AsQueryable();
            if (request.RequestType == "change")
            {
                RequestList = RequestList.Include(x => x.Request!.CorrectionRequest.Event.EventOwener);
            }
            else if (request.RequestType == "authentication")
            {
                RequestList = RequestList.Include(x => x.Request!.CorrectionRequest.Event.EventOwener);
            }
            else if (request.RequestType == "verification")
            {
                RequestList = RequestList.Include(x => x.Request!.VerficationRequest.Event.EventOwener);
            }
            if (request.Status == "approved")
            {
                RequestList = RequestList.Where(r => r.ApprovalStatus == true);
            }
            else if (request.Status == "rejected")
            {
                RequestList = RequestList.Where(r => r.ApprovalStatus == false);
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
            var RequestListDto = RequestList.Where(x => x.Request.RequestType == request.RequestType && x.CivilRegOfficerId == _ResolverService.GetUserId())
             .OrderByDescending(w => w.CreatedAt)
             .Select(t => new AuthenticationRequestListDTO
             {
                 Id = t.Id,
                 OfficerId = Guid.Parse(t.CivilRegOfficerId),
                 RequestedBy = t.Request.CivilRegOfficer.FullNameLang,
                 RequestType = t.Request.RequestType,
                 RequestId = (t.Request.CorrectionRequest == null) ? (t.Request.AuthenticationRequest == null) ?
                     t.Request.PaymentExamptionRequest.Id : _WorkflowService.GetEventId(t.Request.AuthenticationRequest.CertificateId) : t.Request.CorrectionRequest.Id,
                 EventType = (request.RequestType == "authentication")  ? t.Request.AuthenticationRequest.Certificate.Event.EventType :
                             request.RequestType == "change" ? t.Request.CorrectionRequest.Event.EventType :
                                request.RequestType == "verification" ? t.Request.VerficationRequest.Event.EventType : string.Empty,
                 CertificateId = (request.RequestType == "authentication") ? t.Request.AuthenticationRequest!.Certificate!.Event.CertificateId :
                                 request.RequestType == "change" ? t.Request.CorrectionRequest.Event.CertificateId :
                                request.RequestType == "verification" ? t.Request.VerficationRequest.Event.CertificateId : string.Empty,
                                
                 EventOwnerName = (request.RequestType == "authentication") ? t.Request.AuthenticationRequest!.Certificate!.Event.EventOwener.FullNameLang :
                                    request.RequestType == "change" ? t.Request.CorrectionRequest.Event.EventOwener.FullNameLang : 
                                request.RequestType == "verification" ? t.Request.VerficationRequest.Event.EventOwener.FullNameLang : string.Empty,
                 CurrentStep = t.Request.currentStep,
                 NextStep = t.Request.NextStep,
                 RequestDate =new CustomDateConverter(t.Request.CreatedAt).ethiopianDate,
             });
            var List = await PaginatedList<AuthenticationRequestListDTO>
                             .CreateAsync(
                                  RequestListDto
                                 , request.PageCount ?? 1, request.PageSize ?? 10);
            return List;
        }
    }
}
