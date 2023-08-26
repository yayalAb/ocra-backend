

using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Utility.Services;
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
        public string? CreatedDate { get; set; }
        public bool Paid { get; set; } = false;
        public float? Amount { get; set; }
        public CouchPaymentDTO? Payment { get; set; }
        public bool Failed { get; set; } = false;
        public string? FailureMessage { get; set; }
        public bool Exported { get; set; } = false;
        public bool CertificateSynced { get; set; } = false;
        public bool paymentSynced { get; set; } = false;
        public string? serialNo { get; set; }
        [NotMapped]

        public DateTime CreatedDateGorg { get; set; }

        [NotMapped]
        public string? _CreatedDate
        {
            get { return CreatedDate; }
            set
            {
                this.CreatedDate = value;

                CreatedDateGorg = new CustomDateConverter(CreatedDate).gorgorianDate;
            }
        }
    }
}