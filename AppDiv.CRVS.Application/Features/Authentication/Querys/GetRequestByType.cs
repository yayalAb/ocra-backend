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
        public Guid UserId { get; set; }
        public string RequestType { get; set; } = "change";
        public string Status { get; set; } = "inprogress";
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
            var userGroup = _UserRepo.GetAll()
            .Include(g => g.UserGroups)
            .Include(x=>x.Address)
            .Where(x => x.Id == _ResolverService.GetUserId()).FirstOrDefault();
            if (userGroup == null)
            {
                throw new NotFoundException("user does not found");
            }
            var address=await _dateAndAddressService.FormatedAddress(_ResolverService.GetWorkingAddressId());
            var RequestList = _transactionService.GetAllGrid()
                 .Include(x=>x.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress)
                //  .ThenInclude(x=>x.Address)
                //  .ThenInclude(x=>x.ParentAddress)
                //  .ThenInclude(x=>x.ParentAddress)
                //  .ThenInclude(x=>x.ParentAddress)
                //  .Include(w => w.Workflow)
                //  .ThenInclude(ss => ss.Steps)
                //  .ThenInclude(g => g.UserGroup)
                 .AsQueryable();
            if (request.RequestType == "change")
            {
                RequestList = RequestList.Include(x => x.Request!.AuthenticationRequest.Certificate.Event.EventOwener);
            }
            else if (request.RequestType == "authentication")
            {
                RequestList = RequestList.Include(x => x.Request!.CorrectionRequest.Event.EventOwener);
            }
            else if (request.RequestType == "verfication")
            if (request.Status == "inprogress")
            {
                RequestList = RequestList.Where(r => r.Request.currentStep < r.Request.NextStep && r.Request.IsRejected == false);
            }
            else if (request.Status == "approved")
            {
                RequestList = RequestList.Where(r => r.Request.currentStep == r.Request.NextStep);
            }
            else if (request.Status == "rejected")
            {
                RequestList = RequestList.Where(r => r.Request.IsRejected == true);
            }
                 
            // RequestList.Where(wf => ((wf.Workflow.workflowName == wf.RequestType && wf.NextStep != wf.currentStep)));

             if (userGroup.Address.AdminLevel == 1)
            {
                RequestList=RequestList.Where(e => (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 2)
            {
                RequestList=RequestList.Where(e => (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 3)
            {
                RequestList=RequestList.Where(e => (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
               || (e.Request.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 4)
            {
                RequestList=RequestList.Where(e => (e.Request.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
                || (e.Request.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 5)
            {
                RequestList=RequestList.Where(e => e.Request.CivilRegOfficer.ApplicationUser.AddressId== userGroup.AddressId);
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
                 Id = t.Request.Id,
                 ResponsbleGroupId = t.Request.Workflow.Steps.Where(g => g.step == t.Request.NextStep).Select(x => x.UserGroupId).FirstOrDefault() ?? Guid.Empty,
                 ResponsbleGroup = t.Request.Workflow.Steps.Where(g => g.step == t.Request.NextStep).Select(x => x.UserGroup.GroupName).FirstOrDefault(),
                 OfficerId = t.Request.CivilRegOfficerId,
                 RequestedBy = t.Request.CivilRegOfficer.FullNameLang,
                 RequestType = t.Request.RequestType,
                 RequestId = (t.Request.CorrectionRequest == null) ? (t.Request.AuthenticationRequest == null) ?
                     t.Request.PaymentExamptionRequest.Id : _WorkflowService.GetEventId(t.Request.AuthenticationRequest.CertificateId) : t.Request.CorrectionRequest.Id,
                 EventType = (request.RequestType == "authentication")  ? t.Request.AuthenticationRequest.Certificate.Event.EventType :
                                request.RequestType == "change" ? t.Request.CorrectionRequest!.Event.EventType :
                                request.RequestType == "verfication" ? t.Request.VerficationRequest.Event.EventType : string.Empty,
                 CertificateId = request.RequestType == "change" ? t.Request.CorrectionRequest!.Event.CertificateId : 
                                (request.RequestType == "authentication") ? t.Request.AuthenticationRequest!.Certificate!.Event.CertificateId :
                                request.RequestType == "verfication" ? t.Request.VerficationRequest.Event.CertificateId : string.Empty,
                                
                 EventOwnerName = (request.RequestType == "authentication") ? (string?)t.Request.AuthenticationRequest!.Certificate!.Event.EventOwener.FullNameLang :
                                request.RequestType == "change" ? (string?)t.Request.CorrectionRequest!.Event.EventOwener.FullNameLang :
                                request.RequestType == "verfication" ? t.Request.VerficationRequest.Event.EventOwener.FullNameLang : string.Empty,
                 CurrentStep = t.Request.currentStep,
                 NextStep = t.Request.NextStep,
                 RequestDate =new CustomDateConverter(t.Request.CreatedAt).ethiopianDate,
                 CanEdit = ((t.Request.currentStep == 0) && (t.Request.CivilRegOfficerId == userGroup.PersonalInfoId)),
                 CanApprove = userGroup.UserGroups.Select(x => x.Id)
                 .FirstOrDefault() == t.Request.Workflow.Steps.Where(g => g.step == t.Request.NextStep)
                 .Select(x => x.UserGroupId).FirstOrDefault()
             });


            if (!request.IsYourRequestList)
            {
                RequestListDto = RequestListDto.Where(rg => userGroup.UserGroups.Select(g => g.Id).ToList().Contains(rg.ResponsbleGroupId)); 
                // RequestListDto = RequestListDto.Where(rg => rg.ResponsbleGroupId == userGroup.UserGroups.Select(g => g.Id).FirstOrDefault());
            }
            else
            {
                RequestListDto = RequestListDto.Where(rg => rg.OfficerId == userGroup.PersonalInfoId);
            }

            var List = await PaginatedList<AuthenticationRequestListDTO>
                             .CreateAsync(
                                  RequestListDto
                                 , request.PageCount ?? 1, request.PageSize ?? 10);
            if (RequestList == null)
            {
                throw new NotFoundException(" Request does not Exist");
            }
            return List;
        }
    }
}
