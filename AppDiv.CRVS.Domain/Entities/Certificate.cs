using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Certificate : BaseAuditableEntity
    {
        public Guid EventId { get; set; }
        public string ContentStr { get; set; }
        public bool Status { get; set; }
        public bool AuthenticationStatus { get; set; }
        public int PrintCount { get; set; }
        public string CertificateSerialNumber { get; set; }

        [NotMapped]
        public JObject Content
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ContentStr) ? "{}" : ContentStr);
            }
            set
            {
                ContentStr = value.ToString();
            }
        }

        public virtual Event Event { get; set; }

    }
}