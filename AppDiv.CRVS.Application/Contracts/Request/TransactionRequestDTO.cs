using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class TransactionRequestDTO
    {
        public int CurrentStep { get; set; }
        public bool ApprovalStatus { get; set; }
        public Guid WorkflowId { get; set; }
        public Guid RequestId { get; set; }
        public string CivilRegOfficerId { get; set; }
        public string? Remark { get; set; }
        public JArray? RejectionReasons { get; set; }

        public Guid? ReasonLookupId { get; set; }

    }
}