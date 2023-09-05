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
        private readonly ICertificateRepository _certificateRepository;
        private readonly IEventPaymentRequestService _eventPayment;
        public AuthenticationRequestCommadHandler(IEventPaymentRequestService eventPayment, ICertificateRepository certificateRepository, IWorkflowRepository WorkflowRepository, IAuthenticationRepository AuthenticationRepository, IWorkflowService WorkflowService, ITransactionService transactionService, INotificationService notificationService, IUserRepository userRepository)
        {
            _AuthenticationRepository = AuthenticationRepository;
            _WorkflowService = WorkflowService;
            _transactionService = transactionService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _WorkflowRepository = WorkflowRepository;
            _certificateRepository = certificateRepository;
            _eventPayment = eventPayment;
        }
        public async Task<BaseResponse> Handle(AuthenticationRequestCommad request, CancellationToken cancellationToken)
        {
            var response = new BaseResponse();
            var Workflow = _WorkflowRepository.GetAll()
            .Include(x => x.Steps)
            .Where(wf => wf.workflowName == "authentication").FirstOrDefault();
            var AuthenticationRequest = new AuthenticationRequest
            {
                Id=new Guid(),
                CertificateId = request.CertificateId,
                Request = new Request
                {   Id=new Guid(),
                    RequestType = "authentication",
                    CivilRegOfficerId = request.CivilRegOfficer,
                    currentStep = 0,
                    NextStep = 0,
                    WorkflowId = Guid.Empty
                }
            };
            await _AuthenticationRepository.InsertAsync(AuthenticationRequest, cancellationToken);

            if (Workflow == null || Workflow?.Steps.Count == 0)
            {
                var certificate = _certificateRepository.GetAll()
                .Include(x => x.Event)
                .Include(x => x.Event.EventOwener)
                .Where(x => x.Id == request.CertificateId).FirstOrDefault();

                if (certificate == null)
                {
                    throw new NotFoundException("Certificate With the given Id Does't Found");
                }                
                (float amount, string code) respons = await _eventPayment.CreatePaymentRequest(certificate.Event.EventType, certificate.Event, "authentication",
                    AuthenticationRequest?.Request?.Id, false, false, cancellationToken);

                if (true)
                {
                    certificate.AuthenticationStatus = true;
                    certificate.AuthenticationAt = DateTime.Now;
                    await _certificateRepository.UpdateAsync(certificate, x => x.Id);
                    await _certificateRepository.SaveChangesAsync(cancellationToken);
                    response.Message = "Requested Certificate  authenticated";
                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Message = "Payment Request Sent";
                    response.Success = true;
                    return response;
                }

            }
            var next = _WorkflowService.GetNextStep("authentication", 0, true);
            AuthenticationRequest.Request.NextStep=next;
            AuthenticationRequest.Request.WorkflowId=Workflow.Id;

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
                WorkflowId = Workflow.Id,
                RequestId = AuthenticationRequest.RequestId,
                CivilRegOfficerId = userId,//_UserResolverService.GetUserId().ToString(),
                Remark = request.Remark
            };
            await _transactionService.CreateTransaction(NewTranscation);
            var eventId = _certificateRepository.GetAll()
                           .Where(x => x.Id == request.CertificateId).Select(c => c.EventId).FirstOrDefault();
            if (eventId != null)
            {

                await _notificationService.CreateNotification(eventId, "Authentication", request.Remark,
                                   _WorkflowService.GetReceiverGroupId("Authentication", (int)AuthenticationRequest.Request.NextStep), AuthenticationRequest.RequestId,
                                 userId);
            }

            response.Message = "Authentication Request Sent Sucessfully";
            response.Success = true;

            return response;
        }
    }
}
