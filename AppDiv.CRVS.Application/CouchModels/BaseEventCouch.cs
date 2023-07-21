

using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class BaseEventCouch : CouchDocument
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid Id2 { get; set; }
        public string EventType { get; set; }
        public bool Synced { get; set; }
        // public bool? Updated {get; set;}
        public bool Certified { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool Paid { get; set; } = false;
        public float? Amount { get; set; }
        public CouchPaymentDTO? Payment { get; set; }
        public bool Failed {get; set; }= false;
        public string? FailureMessage {get;set;}  

    }
}