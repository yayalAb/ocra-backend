using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Payment : BaseAuditableEntity
    {
        public Guid PaymentRequestId { get; set; }
        public Guid PaymentWayLookupId { get; set;}
        public string BillNumber {get; set; }
        public PaymentRequest PaymentRequest {get; set; }
        public Lookup PaymentWayLookup { get; set; }

    }
}