
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class ProfileChangeRequest : BaseAuditableEntity
    {
        public string? Remark { get; set; }
        public string UserId { get; set; }
        public bool RequestStatus { get; set; } = false;
        public string ContentStr { get; set; }
        public Guid RequestId { get; set; }
        [NotMapped]
        public JObject Content
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ContentStr) ? "{}" : ContentStr);
            }
            set
            {
                ContentStr =  value.ToString();
            }
        }

        public virtual ApplicationUser User { get; set; }
        public virtual Request Request { get; set; }

    }
}