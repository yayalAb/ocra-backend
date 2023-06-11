using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventRequestBase
    {
        public Guid? Id { get; set; }
        public string? RegBookNo { get; set; }
        public string? CivilRegOfficeCode { get; set; }
        public string? EventType { get; set; }
        public string CertificateId { get; set; }
        public string? EventDateEt { get; set; }
        public string? EventRegDateEt { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public ICollection<AddSupportingDocumentRequest> EventSupportingDocuments { get; set; }
        public AddPaymentExamptionRequest? PaymentExamption { get; set; }
    }
}
