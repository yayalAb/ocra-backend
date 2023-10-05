
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Request : BaseAuditableEntity
    {
        public string RequestType { get; set; }
        public Guid CivilRegOfficerId { get; set; }

        public int currentStep { get; set; }
        public int? NextStep { get; set; }
        public Guid WorkflowId { get; set; }
        public bool? IsRejected { get; set; }=false;
        public virtual PersonalInfo CivilRegOfficer { get; set; }
        public virtual CorrectionRequest CorrectionRequest { get; set; }
        public virtual AuthenticationRequest AuthenticationRequest { get; set; }
        public virtual VerficationRequest VerficationRequest { get; set; }
        public virtual ProfileChangeRequest ProfileChangeRequest {get;set;}
        public virtual PaymentExamptionRequest PaymentExamptionRequest { get; set; }
        public virtual PaymentRequest PaymentRequest { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Notification Notification { get; set; }
        public virtual Workflow Workflow { get; set; }
        public bool isDeleted  { get; set; }=false;

    }
}