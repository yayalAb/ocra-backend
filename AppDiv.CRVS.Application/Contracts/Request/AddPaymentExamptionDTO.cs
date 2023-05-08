using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class AddPaymentExamptionDTO
    {
        public Guid ExamptionRequestId { get; set; }
        public  ICollection<AddSupportingDocumentRequest> SupportingDocuments { get; set; }

    }
}