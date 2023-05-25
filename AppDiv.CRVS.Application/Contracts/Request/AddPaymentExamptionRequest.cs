using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddPaymentExamptionRequest
    {
        public Guid? Id { get; set; } = null;
        public Guid ExamptionRequestId { get; set; }
        public ICollection<AddSupportingDocumentRequest>? SupportingDocuments { get; set; }
    }
}