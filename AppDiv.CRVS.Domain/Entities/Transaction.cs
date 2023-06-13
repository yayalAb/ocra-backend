using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Transaction : BaseAuditableEntity
    {
        //    public string OldValueStr {get; set;}
        //    public string NewValueStr { get; set; }
        public int CurrentStep { get; set; }
        public bool ApprovalStatus { get; set; }
        public Guid WorkflowId { get; set; }
        public Guid RequestId { get; set; }
        public string? CivilRegOfficerId { get; set; }
        public string? Remark { get; set; }
        public virtual Workflow? Workflow { get; set; }
        public virtual Request? Request { get; set; }
        public virtual ApplicationUser? CivilRegOfficer { get; set; }


    }
}