
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CorrectionRequest : BaseAuditableEntity
    {
        public string? DescriptionStr { get; set; }
        public Guid EventId { get; set; }
        public bool RequestStatus { get; set; } = false;
        public string ContentStr { get; set; }
        public int currentStep { get; set; }
        public Guid RequestId { get; set; }

        [NotMapped]
        public JObject Description
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(DescriptionStr) ? "{}" : DescriptionStr);
            }
            set
            {
                DescriptionStr = value.ToString();
            }
        }
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
        public virtual Request Request { get; set; }

    }
}