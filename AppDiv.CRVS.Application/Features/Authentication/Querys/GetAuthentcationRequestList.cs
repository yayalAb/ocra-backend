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
    public class GetAuthentcationRequestList : IRequest<object>
    {
        public string RequestType { get; set; } = "change";
        public bool IsYourRequestList { get; set; } = false;
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;
        public string? SearchString { get; set; }


    }
    public class GetAuthentcationRequestListHandler : IRequestHandler<GetAuthentcationRequestList, object>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IWorkflowRepository _WorkflowRepo;
        private readonly IRequestRepostory _RequestRepostory;
        private readonly IUserRepository _UserRepo;
        private readonly IUserResolverService _ResolverService;
        private readonly IDateAndAddressService _dateAndAddressService;


        public GetAuthentcationRequestListHandler(IDateAndAddressService dateAndAddressService, IUserRepository UserRepo, IUserResolverService ResolverService, IWorkflowRepository WorkflowRepo, IRequestRepostory RequestRepostory, IWorkflowService WorkflowService)
        {
            _WorkflowService = WorkflowService;
            _RequestRepostory = RequestRepostory;
            _WorkflowRepo = WorkflowRepo;
            _UserRepo = UserRepo;
            _ResolverService = ResolverService;
            _dateAndAddressService=dateAndAddressService;
        }
        public async Task<object> Handle(GetAuthentcationRequestList request, CancellationToken cancellationToken)
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
            var RequestList = _RequestRepostory.GetAllQueryableAsync()
                 .Include(x => x.CivilRegOfficer)
                 .ThenInclude(x=>x.ApplicationUser)
                 .ThenInclude(x=>x.Address)
                 .ThenInclude(x=>x.ParentAddress)
                 .ThenInclude(x=>x.ParentAddress)
                 .ThenInclude(x=>x.ParentAddress)
                 .Include(x => x.AuthenticationRequest)
                 .ThenInclude(x => x.Certificate)
                 .ThenInclude(x => x.Event)
                 .ThenInclude(x => x.EventOwener)
                 .Include(x => x.CorrectionRequest)
                 .ThenInclude(x => x.Event.EventOwener)
                 .Include(w => w.Workflow)
                 .ThenInclude(ss => ss.Steps)
                 .ThenInclude(g => g.UserGroup)
              .Where(wf => ((wf.Workflow.workflowName == wf.RequestType && wf.NextStep != wf.currentStep)
               &&(wf.PaymentRequest==null)&&(wf.CorrectionRequest!=null||wf.AuthenticationRequest!=null)));

             if (userGroup.Address.AdminLevel == 1)
            {
                RequestList=RequestList.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 2)
            {
                RequestList=RequestList.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 3)
            {
                RequestList=RequestList.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
               || (e.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 4)
            {
                RequestList=RequestList.Where(e => (e.CivilRegOfficer.ApplicationUser.Address.ParentAddress.Id == userGroup.AddressId)
                || (e.CivilRegOfficer.ApplicationUser.Address.Id == userGroup.AddressId));
            }
            else if (userGroup.Address.AdminLevel == 5)
            {
                RequestList=RequestList.Where(e => e.CivilRegOfficer.ApplicationUser.AddressId== userGroup.AddressId);
            }
            if (!string.IsNullOrEmpty(request.SearchString))
            {
                RequestList = RequestList.Where(
                    u => EF.Functions.Like(u.CivilRegOfficer.FirstNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CivilRegOfficer.MiddleNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CivilRegOfficer.LastNameStr!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.RequestType!, "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.currentStep.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.CreatedAt.ToString(), "%" + request.SearchString + "%") ||
                         EF.Functions.Like(u.NextStep.ToString()!, "%" + request.SearchString + "%"));
            }
            var RequestListDto = RequestList.Where(x => (x.RequestType == request.RequestType) && x.IsRejected==false)
             .OrderByDescending(w => w.CreatedAt)
             .Select(w => new AuthenticationRequestListDTO
             {
                 Id = w.Id,
                 ResponsbleGroupId = w.Workflow.Steps.Where(g => g.step == w.NextStep).Select(x => x.UserGroupId).FirstOrDefault() ?? Guid.Empty,
                 ResponsbleGroup = w.Workflow.Steps.Where(g => g.step == w.NextStep).Select(x => x.UserGroup.GroupName).FirstOrDefault(),
                 OfficerId = w.CivilRegOfficerId,
                 RequestedBy = w.CivilRegOfficer.FullNameLang,
                 RequestType = w.RequestType,
                 EventId = (request.RequestType == "authentication")  ? w.AuthenticationRequest.Certificate.EventId :
                             request.RequestType == "change" ? w.CorrectionRequest.EventId : null,
                 RequestId = (w.CorrectionRequest == null) ? (w.AuthenticationRequest == null) ?
                     w.PaymentExamptionRequest.Id : w.AuthenticationRequest.Certificate.EventId : w.CorrectionRequest.Id,
                 EventType = (w.AuthenticationRequest!.Certificate != null) ? (string)w.AuthenticationRequest.Certificate.Event.EventType :
                                (string)w.CorrectionRequest.Event.EventType,
                 CertificateId = request.RequestType == "change" ? w.CorrectionRequest.Event.CertificateId : 
                                request.RequestType == "authentication" ? w.AuthenticationRequest.Certificate.Event.CertificateId : "",
                                
                 EventOwnerName = request.RequestType == "authentication" ? w.AuthenticationRequest.Certificate.Event.EventOwener.FullNameLang :
                                w.CorrectionRequest.Event.EventOwener.FullNameLang,
                 CurrentStep = w.currentStep,
                 NextStep = w.NextStep,
                 RequestDate =new CustomDateConverter(w.CreatedAt).ethiopianDate,
                 CanEdit = ((w.currentStep == 0) && (w.CivilRegOfficerId == userGroup.PersonalInfoId)),
                 CanApprove = userGroup.UserGroups.Select(x => x.Id)
                 .FirstOrDefault() == w.Workflow.Steps.Where(g => g.step == w.NextStep)
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
