
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        public string Type { get; set; }
        public string MessageStr { get; set; }
        public Guid RequestId {get; set;}
        public Guid GroupId { get; set; }


        [NotMapped]
        public JObject Message
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(MessageStr) ? "{}" : MessageStr);
            }
            set
            {
                MessageStr = value.ToString();
            }
        }

        public virtual UserGroup UserGroup { get; set; }
        public virtual Request Request { get; set; }

    }
}