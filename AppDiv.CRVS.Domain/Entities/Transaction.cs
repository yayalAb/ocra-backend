using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Transaction : BaseAuditableEntity
    {
        public int CurrentStep { get; set; }
        public bool ApprovalStatus { get; set; }
        public Guid WorkflowId { get; set; }
        public Guid RequestId { get; set; }
        public string? CivilRegOfficerId { get; set; }
        public string? Remark { get; set; }
        public string? RejectionReasonsStr { get; set; }
        public Guid? ReasonLookupId { get; set; }
        public virtual Workflow? Workflow { get; set; }
        public virtual Request? Request { get; set; }
        public virtual ApplicationUser? CivilRegOfficer { get; set; }
        public virtual Lookup ReasonLookup { get; set; }
        
        [NotMapped]
        public JArray RejectionReasonsArray
        {
            set 
            {
                RejectionReasonsStr = value.ToString();
            }
            get
            {
                return JArray.Parse(RejectionReasonsStr ?? "[]");
            }
        }

    }
}