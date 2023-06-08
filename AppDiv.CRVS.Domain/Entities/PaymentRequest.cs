using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PaymentRequest : BaseAuditableEntity
    {
        public string? ReasonStr { get; set; }
        public Guid? EventId { get; set; }
        public Guid? RequestId { get; set; }
        public float Amount { get; set; }
        public bool status { get; set; }
        public Guid PaymentRateId { get; set; }
        public string? PaymentCode { get; set; }
        public virtual Event Event { get; set; }
        public virtual PaymentRate PaymentRate { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual Request Request { get; set; }
        [NotMapped]
        public JObject Reason
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ReasonStr) ? "{}" : ReasonStr);
            }
            set
            {
                ReasonStr = value.ToString();
            }
        }

    }


}
