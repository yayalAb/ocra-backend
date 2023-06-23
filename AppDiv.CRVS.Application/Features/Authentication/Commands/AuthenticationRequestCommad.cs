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

namespace AppDiv.CRVS.Application.Features.Authentication.Commands
{
    public class AuthenticationRequestCommad : IRequest<BaseResponse>
    {
        public Guid CertificateId { get; set; }
        public Guid CivilRegOfficer { get; set; }

        public string? Remark { get; set; }
    }
    public class AuthenticationRequestCommadHandler : IRequestHandler<AuthenticationRequestCommad, BaseResponse>
    {
        private readonly IAuthenticationRepository _AuthenticationRepository;
        private readonly IWorkflowService _WorkflowService;
        private readonly ITransactionService _transactionService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IWorkflowRepository _WorkflowRepository;
        public AuthenticationRequestCommadHandler(IWorkflowRepository WorkflowRepository, IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService, ITransactionService transactionService, INotificationService notificationService, IUserRepository userRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _WorkflowRepository = WorkflowRepository;
        }
        public async Task<BaseResponse> Handle(AuthenticationRequestCommad request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            Guid WorkflowId = _WorkflowRepository.GetAll()
            .Where(wf => wf.workflowName == "authentication").Select(x => x.Id).FirstOrDefault();
            if (WorkflowId == null || WorkflowId == Guid.Empty)
            {
                response.Message = "authentication Work Flow Does not exist Pleace Create Workflow First";
                response.Success = false;
                return response;

            }
            var next = _WorkflowService.GetNextStep("authentication", 0, true);
            var AuthenticationRequest = new AuthenticationRequest
            {
                CertificateId = request.CertificateId,
                Request = new Request
                {
                    RequestType = "authentication",
                    CivilRegOfficerId = request.CivilRegOfficer,
                    currentStep = 0,
                    NextStep = next,
                    WorkflowId = WorkflowId
                }
            };
            await _AuthenticationRepository.InsertAsync(AuthenticationRequest, cancellationToken);
            await _AuthenticationRepository.SaveChangesAsync(cancellationToken);
            string? userId = _userRepository.GetAll()
                                .Where(u => u.PersonalInfoId == request.CivilRegOfficer)
                                .Select(u => u.Id).FirstOrDefault();
            if (userId == null)
            {
                response.Message = "user not Found";
                response.Success = false;
                return response;
            }
            var NewTranscation = new TransactionRequestDTO
            {
                CurrentStep = 0,
                ApprovalStatus = true,
                WorkflowId = WorkflowId,
                RequestId = AuthenticationRequest.RequestId,
                CivilRegOfficerId = userId,//_UserResolverService.GetUserId().ToString(),
                Remark = request.Remark
            };

            await _transactionService.CreateTransaction(NewTranscation);
            await _notificationService.CreateNotification(AuthenticationRequest.Request.Id, "Authentication", request.Remark,
                               _WorkflowService.GetReceiverGroupId("Authentication", (int)AuthenticationRequest.Request.NextStep), AuthenticationRequest.RequestId,
                             userId);
            response.Message = "Authentication Request Sent Sucessfully";
            response.Success = true; ;

            return response;
        }
    }
}