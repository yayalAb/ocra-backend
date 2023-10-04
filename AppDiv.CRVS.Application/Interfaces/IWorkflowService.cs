using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces
{
    public interface IWorkflowService
    {
        public int GetLastWorkflow(string workflowType);
        public int GetNextStep(string workflowType, int step, bool isApprove);
        public Task<(bool, Guid)> ApproveService(Guid RequestId, string workflowType, bool IsApprove, string? Remark, JArray? RejectionReasons, Guid? ReasonLookupId, bool paymentAdded, CancellationToken cancellationToken);
        public Guid GetReceiverGroupId(string workflowType, int step);
        public Guid? GetEventId(Guid Id);
        public (bool,bool) WorkflowHasPayment(string workflow, int Step, Guid RequestId);
    }
}