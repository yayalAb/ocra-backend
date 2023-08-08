using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventRequestBase
    {
        public Guid? Id { get; set; }
        public string? RegBookNo { get; set; }
        public string? CivilRegOfficeCode { get; set; }
        public string? EventType { get; set; }
        public string? CertificateId { get; set; }
        public string? EventDateEt { get; set; }
        public string? EventRegDateEt { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public Guid? EventRegisteredAddressId { get; set; }
        public ICollection<AddSupportingDocumentRequest>? EventSupportingDocuments { get; set; }
        public object fingerPrints { get; set; } = new Object{};

        public AddPaymentExamptionRequest? PaymentExamption { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
