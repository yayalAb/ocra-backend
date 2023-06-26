using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class UpdateSupportingDocumentRequest
    {
        public Guid Id { get; set; }
        public Guid? EventId { get; set; }
        public Guid? PaymentExamptionId { get; set;}
        public string? Description { get; set; }
        public string Type {get; set;}
        public string Label {get; set; }
        public string base64String { get; set; }


    }
}