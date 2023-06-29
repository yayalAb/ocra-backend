using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PaymentExamption : BaseAuditableEntity
    {
        public Guid ExamptionReasonLookupId { get; set; }
        public Guid EventId { get; set; }
        public virtual Lookup ExamptionReasonLookup { get; set; }
        public virtual Event Event { get; set; }
        public virtual ICollection<SupportingDocument> SupportingDocuments { get; set; }
    }
}