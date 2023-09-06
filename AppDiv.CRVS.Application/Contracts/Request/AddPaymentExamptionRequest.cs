using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddPaymentExamptionRequest
    {
        public Guid? Id { get; set; } = null;
        public Guid? ExamptionReasonLookupId { get; set; }
        public ICollection<AddSupportingDocumentRequest>? SupportingDocuments { get; set; }
        public DateTime? CreatedAt {get; set; } = DateTime.Now;
        public Guid? CreatedBy {get;set; }
    }
}