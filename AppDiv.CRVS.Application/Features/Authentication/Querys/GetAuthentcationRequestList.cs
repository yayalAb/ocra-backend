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
        private readonly IStepRepository _StepRepo;

        public GetAuthentcationRequestListHandler(IStepRepository StepRepo, IWorkflowRepository WorkflowRepo, IRequestRepostory RequestRepostory, IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
            _RequestRepostory = RequestRepostory;
            _WorkflowRepo = WorkflowRepo;
            _StepRepo = StepRepo;
        }
        public async Task<object> Handle(GetAuthentcationRequestList request, CancellationToken cancellationToken)
        {
            var getAllSteps = _StepRepo.GetAll();//.Where(g => g.UserGroupId == request.UserId);
            var RequestList = _RequestRepostory.GetAll()
            .Include(x => x.CivilRegOfficer)
            .Include(x => x.AuthenticationRequest)
            .Include(x => x.CorrectionRequest);
            var joinedList = RequestList.Join(getAllSteps, r => r.currentStep, w => w.step, (r, w) =>
            new AuthenticationRequestListDTO
            {
                Id = r.Id,
                ResponsbleGroup = w.UserGroup.GroupName,
                ResponsbleGroupId = w.UserGroupId,
                RequestedBy = r.CivilRegOfficer.FirstNameLang + " " + r.CivilRegOfficer.MiddleNameLang + " " + r.CivilRegOfficer.LastNameLang,
                RequestType = r.RequestType,
                RequestId = (r.CorrectionRequest.Id == null) ? r.AuthenticationRequest.CertificateId : r.CorrectionRequest.Id,
                CurrentStep = r.currentStep,
                RequestDate = r.CreatedAt
            });
            var List = await PaginatedList<AuthenticationRequestListDTO>
                             .CreateAsync(
                                  joinedList
                                 , request.PageCount ?? 1, request.PageSize ?? 10);
            if (RequestList == null)
            {
                throw new Exception("Authentication Request not Exist");
            }
            return List;
        }
    }
}