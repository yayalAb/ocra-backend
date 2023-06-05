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
        public Guid CivilRegOfficerId { get; set; }
        public string Remark { get;}

    }
}