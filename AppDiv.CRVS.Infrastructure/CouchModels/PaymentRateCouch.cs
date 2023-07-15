
using CouchDB.Driver.Types;


namespace AppDiv.CRVS.Infrastructure.CouchModels
{
    public class PaymentRateCouch :CouchDocument
    {
        public Guid Id { get; set; }    
        public Guid? PaymentTypeLookupId { get; set; }
        public Guid? EventLookupId { get; set; }
        public string? EventLookupAm {get;set;}
        public string? EventLookupOr {get;set;}
        public string? PaymentTypeLookupAm {get; set; }
        public string? PaymentTypeLookupOr {get; set; }
        public float Amount { get; set; } =0;
        public float Backlog { get; set; } = 0;
        public float HasCamera { get; set; } = 0;
        public float HasVideo { get; set; } = 0;
        public bool Status { get; set; }
        public bool IsForeign { get; set; }
        public bool DeletedStatus {get; set; }
    }
}