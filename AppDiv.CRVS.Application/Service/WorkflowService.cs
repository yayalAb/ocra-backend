
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Application.Service
{
    public class WorkflowService : IWorkflowService
    {
        private readonly IWorkflowRepository _workflowRepository;
        private readonly IStepRepository _stepRepostory;
        private readonly IRequestRepostory _requestRepostory;
        private readonly ITransactionService _TransactionService;
        private readonly IUserResolverService _UserResolverService;
        private readonly INotificationService _NotificationService;
        private readonly ICertificateRepository _CertificateRepository;

        public WorkflowService(ICertificateRepository CertificateRepository, INotificationService NotificationService, IUserResolverService UserResolverService, ITransactionService TransactionService, IWorkflowRepository workflowRepository, IRequestRepostory requestRepostory, IStepRepository stepRepostory)
        {
            _workflowRepository = workflowRepository;
            _stepRepostory = stepRepostory;
            _requestRepostory = requestRepostory;
            _TransactionService = TransactionService;
            _UserResolverService = UserResolverService;
            _CertificateRepository = CertificateRepository;
        }
        public int GetLastWorkflow(string workflowType)
        {
            var lastStep = _stepRepostory.GetAll()
            .Include(x => x.workflow)
            .Where(x => x.workflow.workflowName == workflowType)
            .OrderByDescending(x => x.step).FirstOrDefault();
            return lastStep.step;
        }
        public int GetNextStep(string workflowType, int step, bool isApprove)
        {
            if (isApprove)
            {
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
                throw new Exception("user group not found");
            }
            return (Guid)groupId;
        }

        public async Task<(bool, Guid)> ApproveService(Guid RequestId, string workflowType, bool IsApprove, string? Remark, CancellationToken cancellationToken)
        {
            var request = _requestRepostory.GetAll()
            .Include(x => x.AuthenticationRequest).ThenInclude(a => a.Certificate)
            .Include(x => x.CorrectionRequest)
            .Where(x => x.Id == RequestId).FirstOrDefault();
            Guid ReturnId = (request?.AuthenticationRequest?.Id == null || request?.AuthenticationRequest?.Id == Guid.Empty) ? request.CorrectionRequest.EventId : request.AuthenticationRequest.CertificateId;
            if (request == null)
            {
                throw new Exception("Request Does not Found");
            }
            if (request.currentStep >= 0 && request.currentStep < this.GetLastWorkflow(workflowType))
            {
                var nextStep = this.GetNextStep(workflowType, request.currentStep, IsApprove);
                request.currentStep = nextStep;

                var NewTranscation = new TransactionRequestDTO
                {
                    CurrentStep = request.currentStep,
                    ApprovalStatus = IsApprove,
                    WorkflowId = RequestId,
                    RequestId = RequestId,
                    CivilRegOfficerId = "4d940006-b21f-4841-b8dd-02957c4d7487",
                    Remark = Remark
                };
                await _TransactionService.CreateTransaction(NewTranscation);
                await _NotificationService.CreateNotification(ReturnId, workflowType, workflowType,
                                   this.GetReceiverGroupId(workflowType, request.currentStep), RequestId,
                                  "4d940006-b21f-4841-b8dd-02957c4d7487");
                try
                {
                    await _requestRepostory.UpdateAsync(request, x => x.CreatedBy);
                    await _requestRepostory.SaveChangesAsync(cancellationToken);
                }
                catch (Exception exp)
                {
                    throw new ApplicationException(exp.Message);
                }
            }
            else
            {
                throw new Exception("Next Step  Does not Exist");
            }
            return ((this.GetLastWorkflow(workflowType) == request.currentStep), ReturnId);

        }



        public Guid GetEventId(Guid Id)
        {
            var eventId = _CertificateRepository.GetAll().Where(x => x.Id == Id).FirstOrDefault();
            return eventId.EventId;
        }
    }
}