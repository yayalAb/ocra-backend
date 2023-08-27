using System;
using System.Security.Cryptography.X509Certificates;

using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Exceptions;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Service
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IStepRepository _stepRepostory;
        private readonly IRequestRepostory _requestRepostory;
        // private readonly IWorkflowService _workflowService;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionService _TransactionService;
        private readonly IUserResolverService _UserResolverService;
        private readonly INotificationService _NotificationService;
        private readonly ICertificateRepository _CertificateRepository;
        private readonly INotificationService notificationService;
        private readonly IEventPaymentRequestService _paymentRequestService;
        private readonly IPaymentRequestRepository _paymentRequestRepository;


        private readonly IEventRepository _EventRepository;
        public WorkflowService(IEventRepository EventRepository,
                               IPaymentRequestRepository paymentRequestRepository,
                               IEventPaymentRequestService paymentRequestService,
                               ICertificateRepository CertificateRepository,
                               INotificationService NotificationService,
                               IUserResolverService UserResolverService,
                               ITransactionService TransactionService,
                               IWorkflowRepository workflowRepository,
                               IRequestRepostory requestRepostory,
                               //    IWorkflowService workflowService ,
                               IUserRepository userRepository,
                               IStepRepository stepRepostory)
        {
            _workflowRepository = workflowRepository;
            _stepRepostory = stepRepostory;
            _requestRepostory = requestRepostory;
            // _workflowService = workflowService;
            _userRepository = userRepository;
            _TransactionService = TransactionService;
            _UserResolverService = UserResolverService;
            _CertificateRepository = CertificateRepository;
            notificationService = NotificationService;
            _paymentRequestService = paymentRequestService;
            _EventRepository = EventRepository;
            _paymentRequestRepository = paymentRequestRepository;

        }
        public int GetLastWorkflow(string workflowType)
        {
            var lastStep = _stepRepostory.GetAll();
            var ll = lastStep.Include(x => x.workflow)
            .Where(x => x.workflow.workflowName == workflowType)
            .OrderByDescending(x => x.step).FirstOrDefault();
            return ll.step;
        }
        public int GetNextStep(string workflowType, int step, bool isApprove)
        {
            if (isApprove)
            {
                if (step == this.GetLastWorkflow(workflowType))
                {
                    return step;
                }
                var nextStep = _stepRepostory.GetAll()
                            .Include(x => x.workflow)
                            .Where(x => x.workflow.workflowName == workflowType && x.step > step)
                            .OrderBy(x => x.step).FirstOrDefault();

                return nextStep.step;
            }
            else
            {
                if (step == 1 || step == 0)
                {
                    return 0;
                }
                var nextStep = _stepRepostory.GetAll()
                           .Include(x => x.workflow)
                           .Where(x => x.workflow.workflowName == workflowType && x.step < step)
                           .OrderByDescending(x => x.step).FirstOrDefault();
                return nextStep.step;
            }
        }
        public Guid GetReceiverGroupId(string workflowType, int step)
        {
            var groupId = _workflowRepository.GetAll()
            .Where(w => w.workflowName == workflowType)
            .Select(w => w.Steps.Where(s => s.step == step).Select(s => s.UserGroupId).FirstOrDefault()
            ).FirstOrDefault();
            if (groupId == null)
            {
                throw new NotFoundException("user group not found");
            }
            return (Guid)groupId;
        }
        public async Task<(bool, Guid)> ApproveService(Guid RequestId, string workflowType, bool IsApprove, string? Remark, Guid? ReasonLookupId, bool paymentAdded, CancellationToken cancellationToken)
        {
            var request = _requestRepostory.GetAll()
            .Include(x => x.AuthenticationRequest).ThenInclude(a => a.Certificate)
            .Include(x => x.CorrectionRequest)
            .Include(x => x.VerficationRequest)
            .Include(x => x.Notification)
            .Include(x => x.PaymentExamptionRequest)
            .Include(x => x.VerficationRequest)
            .Where(x => x.Id == RequestId).FirstOrDefault();
            if (request == null)
            {
                throw new NotFoundException("Request Does not Found");
            }
            Guid ReturnId = Guid.Empty;
            if (request.RequestType == "verification")
            {
                ReturnId = request.VerficationRequest.EventId;
            }
            else
            {
                ReturnId = (request?.AuthenticationRequest?.Id == null || request?.AuthenticationRequest?.Id == Guid.Empty) ?
                  (request?.CorrectionRequest?.EventId == null || request?.CorrectionRequest?.EventId == Guid.Empty) ?
                  request.PaymentExamptionRequest.Id : request.CorrectionRequest.EventId : request.AuthenticationRequest.CertificateId;
            }

            if (request.currentStep >= 0 && request.currentStep < this.GetLastWorkflow(workflowType))
            {
                var nextStep = this.GetNextStep(workflowType, request.currentStep, IsApprove);
                bool nextStep1 = false;
                var checkPayment = this.WorkflowHasPayment(workflowType, nextStep, RequestId);
                if (checkPayment.Item1 && !paymentAdded)
                {
                    if (checkPayment.Item2)
                    {
                        return (false, Guid.Empty);
                    }
                    (float?, string) res = await this.CreatePaymentRequest(workflowType, RequestId, cancellationToken);
                    if (res.Item1 != 0 || res.Item1 != 0.0)
                    {
                        return (false, Guid.Empty);
                    }

                }
                try
                {
                    string? userId = _userRepository.GetAll()
                                        .Where(u => u.PersonalInfoId == _UserResolverService.GetUserPersonalId())
                                        .Select(u => u.Id).FirstOrDefault();

                    if (string.IsNullOrEmpty(userId))
                    {
                        throw new NotFoundException("user not found");
                    }
                    if (!IsApprove && request.currentStep == 0)
                    {
                        request.IsRejected = true;
                    }
                    request.currentStep = nextStep;
                    request.NextStep = this.GetNextStep(workflowType, nextStep, true);
                    if (request.Notification != null)
                    {
                        request.Notification.MessageStr = Remark;
                        request.Notification.GroupId = this.GetReceiverGroupId(workflowType, (int)request.NextStep);
                        request.Notification.SenderId = userId;
                    }

                    _requestRepostory.Update(request);
                    _requestRepostory.SaveChanges();
                    var NewTranscation = new TransactionRequestDTO
                    {
                        CurrentStep = 0,
                        ApprovalStatus = true,
                        WorkflowId = request.WorkflowId,
                        RequestId = request.Id,
                        CivilRegOfficerId = userId,
                        Remark = Remark,
                        ReasonLookupId = ReasonLookupId
                    };

                    await _TransactionService.CreateTransaction(NewTranscation);

                    //send notification
                    Guid? notificationObjId = request.CorrectionRequest != null
                                           ? request.CorrectionRequest.Id
                                           : request.AuthenticationRequest != null
                                           ?
                                           _CertificateRepository
                                           .GetAll()
                                           .Where(c => c.Id == request.AuthenticationRequest.CertificateId)
                                           .Select(c => c.EventId).FirstOrDefault()
                                           : request.VerficationRequest.EventId;


                    if (notificationObjId != null)
                    {
                        await notificationService.CreateNotification((Guid)notificationObjId, workflowType!, Remark ?? "",
                                        this.GetReceiverGroupId(workflowType, (int)request.NextStep), request.Id,
                                      userId);

                    }

                    //update old notification seen status to true
                    if (request.Notification?.Id != null)
                    {
                        await notificationService.updateSeenStatus(request.Notification.Id);

                    }


                }
                catch (Exception exp)
                {
                    throw new NotFoundException(exp.Message);
                }
            }
            else
            {
                return (true, ReturnId);
            }
            return (this.GetLastWorkflow(workflowType) == request.currentStep, ReturnId);

        }
        public Guid? GetEventId(Guid Id)
        {
            var eventId = _CertificateRepository.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            if (eventId == null)
            {
                throw new NotFoundException("event not found");
            }
            return eventId.EventId;
        }

        public (bool, bool) WorkflowHasPayment(string workflow, int Step, Guid RequestId)
        {
            var requestHaspayment = _requestRepostory.GetAll()
            .Include(x => x.Workflow)
            .ThenInclude(x => x.Steps)
            .Include(x => x.PaymentRequest)
            .ThenInclude(x => x.Payment)
            .Where(re => ((re.Id == RequestId && re.Workflow.HasPayment) && (re.Workflow.PaymentStep == Step))
            ).FirstOrDefault();
            if (requestHaspayment != null)
            {
                if (requestHaspayment?.PaymentRequest == null)
                {
                    return (true, false);
                }
                else if (requestHaspayment?.PaymentRequest != null && requestHaspayment?.PaymentRequest.status == false)
                {
                    return (true, true);
                }
            }
            return (false, false);
        }
        public async Task<(float?, string)> CreatePaymentRequest(string workflowType, Guid RequestId, CancellationToken cancellationToken)
        {
            var request = _requestRepostory.GetAll()
            .Include(x => x.AuthenticationRequest)
            .Include(X => X.CorrectionRequest)
            .Include(x => x.PaymentRequest).ThenInclude(p => p.Payment)
            .Where(x => x.Id == RequestId).FirstOrDefault();
            if (request == null)
            {
                throw new NotFoundException("Request Does not Found");
            }

            if ((request.RequestType == "authentication" || request.RequestType == "change") && (request?.PaymentRequest?.Id == null || request?.PaymentRequest?.Id == Guid.Empty))
            {
                try
                {
                    Guid? EventId = (request?.CorrectionRequest?.Id == null) ? this.GetEventId(request.AuthenticationRequest.CertificateId) : request.CorrectionRequest.EventId;
                    var selectedEvent = _EventRepository.GetAll()
                    .Include(x => x.EventOwener)
                    .Where(x => x.Id == EventId).FirstOrDefault();
                    (float? amount, string? code) response = await _paymentRequestService.CreatePaymentRequest(selectedEvent.EventType, selectedEvent, workflowType, RequestId, false, false, cancellationToken);
                    return response;
                }
                catch (Exception ex)
                {
                    throw new NotFoundException("Payment Rate Not found Please Add Payment Rate");
                }

            }
            else
            {
                return (0, "11111");
            }

        }
    }
}