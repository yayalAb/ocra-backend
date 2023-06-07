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
            var userGroup = _UserRepo.GetAll()
            .Include(g => g.UserGroups)
            .Where(x => x.Id == request.UserId.ToString()).FirstOrDefault();

            var gropId = userGroup.UserGroups.FirstOrDefault();
            var RequestList = _RequestRepostory.GetAll()
            .Include(x => x.CivilRegOfficer)
            .Include(x => x.AuthenticationRequest).
             Include(c => c.AuthenticationRequest.Certificate)
            .Include(x => x.CorrectionRequest)
            .Include(w => w.Workflow).ThenInclude(ss => ss.Steps);
            var testVar = RequestList.Where(w => w.Workflow.Steps.Where(g => g.step == w.currentStep && g.UserGroupId == gropId.Id).Any())
            .Select(w => new AuthenticationRequestListDTO
            {
                Id = w.Id,
                ResponsbleGroupId = w.Workflow.Steps.Where(g => g.step == w.currentStep).Select(x => x.UserGroupId).FirstOrDefault(),
                RequestType = w.RequestType,
                RequestId = (w.CorrectionRequest.Id == null) ? _WorkflowService.GetEventId(w.AuthenticationRequest.CertificateId) : w.CorrectionRequest.Id,
                CurrentStep = w.currentStep,
                RequestDate = w.CreatedAt
            });
            var List = await PaginatedList<AuthenticationRequestListDTO>
                             .CreateAsync(
                                  testVar
                                 , request.PageCount ?? 1, request.PageSize ?? 10);
            if (RequestList == null)
            {
                throw new Exception(" Request does not Exist");
            }
            return List;
        }
    }
}