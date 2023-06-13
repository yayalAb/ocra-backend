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

namespace AppDiv.CRVS.Application.Features.Authentication.Commands
{
    public class AuthenticationRequestCommad : IRequest<BaseResponse>
    {
        public Guid CertificateId { get; set; }
        public Guid CivilRegOfficer { get; set; }
    }
    public class AuthenticationRequestCommadHandler : IRequestHandler<AuthenticationRequestCommad, BaseResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly IWorkflowRepository _WorkflowRepository;
        public AuthenticationRequestCommadHandler(IWorkflowRepository WorkflowRepository, IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
            _WorkflowRepository = WorkflowRepository;
        }
        public async Task<BaseResponse> Handle(AuthenticationRequestCommad request, CancellationToken cancellationToken)
        {
            // var NewTranscation = new TransactionRequestDTO
            // {
            //     CurrentStep = request.currentStep,
            //     ApprovalStatus = IsApprove,
            //     WorkflowId = RequestId,
            //     RequestId = RequestId,
            //     CivilRegOfficerId = _UserResolverService.GetUserId().ToString(),
            //     Remark = Remark
            // };
            // await _TransactionService.CreateTransaction(NewTranscation);
            // await _NotificationService.CreateNotification(ReturnId, workflowType, workflowType,
            //                    this.GetReceiverGroupId(workflowType, request.currentStep), RequestId,
            //                   _UserResolverService.GetUserId().ToString());
            Guid WorkflowId = _WorkflowRepository.GetAll()
            .Where(wf => wf.workflowName == "authentication").Select(x => x.Id).FirstOrDefault();
            if (WorkflowId == null || WorkflowId == Guid.Empty)
            {
                throw new Exception("authentication Work Flow Does not exist Pleace Create Workflow First");
            }
            var AuthenticationRequest = new AuthenticationRequest
            {
                CertificateId = request.CertificateId,
                Request = new Request
                {
                    RequestType = "authentication",
                    CivilRegOfficerId = request.CivilRegOfficer,
                    currentStep = 0,
                    NextStep = 1,// _WorkflowService.GetNextStep("authentication", 0, true),
                    WorkflowId = WorkflowId
                }
            };
            await _AuthenticationRepository.InsertAsync(AuthenticationRequest, cancellationToken);
            await _AuthenticationRepository.SaveChangesAsync(cancellationToken);
            return new BaseResponse
            {
                Message = "authentication request Sent"
            };
        }
    }
}