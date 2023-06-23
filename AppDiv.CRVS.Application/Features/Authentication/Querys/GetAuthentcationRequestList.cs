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

namespace AppDiv.CRVS.Application.Features.Authentication.Querys
{
    public class GetAuthentcationRequestList : IRequest<object>
    {
        public Guid UserId { get; set; }
        public int? PageCount { set; get; } = 1!;
        public int? PageSize { get; set; } = 10!;


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
                throw new Exception("Please provide User Id");
            }
            var userGroup = _UserRepo.GetAll()
            .Include(g => g.UserGroups)
            .Where(x => x.Id == request.UserId.ToString()).FirstOrDefault();
            if (userGroup == null)
            {
                throw new Exception("user does not found");
            }
            var RequestList = _RequestRepostory.GetAll()
                 .Include(x => x.CivilRegOfficer)
                 .Include(x => x.AuthenticationRequest)
                 .ThenInclude(x => x.Certificate)
                 .Include(x => x.CorrectionRequest)
                 .Include(x => x.PaymentExamptionRequest)
                 .Include(x => x.PaymentRequest)
                 .Include(w => w.Workflow)
                 .ThenInclude(ss => ss.Steps)
                 .ThenInclude(g => g.UserGroup)
              .Where(wf => wf.Workflow.workflowName == wf.RequestType && wf.NextStep != wf.currentStep)
             .Select(w => new AuthenticationRequestListDTO
             {
                 Id = w.Id,
                 ResponsbleGroupId = w.Workflow.Steps.Where(g => g.step == w.NextStep).Select(x => x.UserGroupId).FirstOrDefault(),
                 ResponsbleGroup = w.Workflow.Steps.Where(g => g.step == w.NextStep).Select(x => x.UserGroup.GroupName).FirstOrDefault(),
                 OfficerId = w.CivilRegOfficerId,
                 RequestedBy = w.CivilRegOfficer.FirstNameLang + " " + w.CivilRegOfficer.MiddleNameLang + " " + w.CivilRegOfficer.LastNameLang,
                 RequestType = w.RequestType,
                 RequestId = (w.CorrectionRequest.Id == null) ? (w.AuthenticationRequest.CertificateId == null) ?
                     w.PaymentExamptionRequest.Id : _WorkflowService.GetEventId(w.AuthenticationRequest.CertificateId) : w.CorrectionRequest.Id,
                 CurrentStep = w.currentStep,
                 NextStep = w.NextStep,
                 RequestDate = w.CreatedAt,
                 CanEdit = ((w.currentStep == 0) && (w.CivilRegOfficerId == userGroup.PersonalInfoId)),
                 CanApprove = userGroup.UserGroups.Select(x => x.Id)
                 .FirstOrDefault() == w.Workflow.Steps.Where(g => g.step == w.NextStep)
                 .Select(x => x.UserGroupId).FirstOrDefault()
             }).Where(rg => rg.ResponsbleGroupId == userGroup.UserGroups.Select(g => g.Id).FirstOrDefault());

            var List = await PaginatedList<AuthenticationRequestListDTO>
                             .CreateAsync(
                                  RequestList
                                 , request.PageCount ?? 1, request.PageSize ?? 10);
            if (RequestList == null)
            {
                throw new Exception(" Request does not Exist");
            }
            return List;
        }
    }
}


//  .Where(wf => wf.CivilRegOfficerId == userGroup.PersonalInfoId)
// Where(w => w.Workflow.Steps.Where(g => g.step == w.currentStep).Any())
//  wf.CivilRegOfficerId == userGroup.PersonalInfoId ||
//   wf.Workflow.Steps.Any(s => s.UserGroupId == new Guid(userGroup.Id))
//  _WorkflowService.GetNextStep(w.RequestType, w.currentStep, true)