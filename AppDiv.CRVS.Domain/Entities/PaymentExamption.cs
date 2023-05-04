using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PaymentExamption : BaseAuditableEntity
    {
        public Guid ExamptionRequestId { get; set; }
        public Guid EventId { get; set; }

        public virtual PaymentExamptionRequest ExamptionRequest { get; set; }
        public virtual Event Event { get; set; }
    }
}