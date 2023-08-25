using System;
using System.Collections.Generic;
using System.Linq;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Common;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Utility.Services;

namespace AppDiv.CRVS.Application.Features.Authentication.Querys
{
    public class GetAuthentcationRequestList : IRequest<object>
    {
        public Guid UserId { get; set; }
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


        public GetAuthentcationRequestListHandler(IUserRepository UserRepo, IUserResolverService ResolverService, IWorkflowRepository WorkflowRepo, IRequestRepostory RequestRepostory, IWorkflowService WorkflowService)
        {
            _WorkflowService = WorkflowService;
            _RequestRepostory = RequestRepostory;
            _WorkflowRepo = WorkflowRepo;
            _UserRepo = UserRepo;
            _ResolverService = ResolverService;
        }
        public async Task<object> Handle(GetAuthentcationRequestList request, CancellationToken cancellationToken)
        {
            if (request.UserId == null || request.UserId == Guid.Empty)
            {
                throw new NotFoundException("Please provide User Id");
            }
            var userGroup = _UserRepo.GetAll()
            .Include(g => g.UserGroups)
            .Where(x => x.Id == request.UserId.ToString()).FirstOrDefault();
            if (userGroup == null)
            {
                throw new NotFoundException("user does not found");
            }
            var RequestList = _RequestRepostory.GetAll()
                 .Include(x => x.CivilRegOfficer)
                 .Include(x => x.AuthenticationRequest)
                 .ThenInclude(x => x.Certificate)
                 .Include(x => x.CorrectionRequest)
                 .Include(x => x.PaymentExamptionRequest)
                 .Include(x => x.PaymentRequest)
                 .Include(x=>x.PaymentRequest)
                 .Include(w => w.Workflow)
                 .ThenInclude(ss => ss.Steps)
                 .ThenInclude(g => g.UserGroup)
              .Where(wf => (wf.Workflow.workflowName == wf.RequestType && wf.NextStep != wf.currentStep)&&(wf.PaymentRequest==null));
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
            var RequestListDto = RequestList.Where(x => (x.RequestType == "change" || x.RequestType == "authentication") && x.IsRejected==false)
             .OrderByDescending(w => w.CreatedAt)
             .Select(w => new AuthenticationRequestListDTO
             {
                 Id = w.Id,
                 ResponsbleGroupId = w.Workflow.Steps.Where(g => g.step == w.NextStep).Select(x => x.UserGroupId).FirstOrDefault() ?? Guid.Empty,
                 ResponsbleGroup = w.Workflow.Steps.Where(g => g.step == w.NextStep).Select(x => x.UserGroup.GroupName).FirstOrDefault(),
                 OfficerId = w.CivilRegOfficerId,
                 RequestedBy = w.CivilRegOfficer.FirstNameLang + " " + w.CivilRegOfficer.MiddleNameLang + " " + w.CivilRegOfficer.LastNameLang,
                 RequestType = w.RequestType,
                 RequestId = (w.CorrectionRequest.Id == null) ? (w.AuthenticationRequest.CertificateId == null) ?
                     w.PaymentExamptionRequest.Id : _WorkflowService.GetEventId(w.AuthenticationRequest.CertificateId) : w.CorrectionRequest.Id,
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
