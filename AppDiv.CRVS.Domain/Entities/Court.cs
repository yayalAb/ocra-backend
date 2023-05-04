using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Court : BaseAuditableEntity
    {
        public String NameStr { get; set; }
        public Guid? AddressId { get; set; }
        public string? DescriptionStr { get; set; }

        public virtual Address Address { get; set; }
        [NotMapped]
        public JObject Name
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(NameStr) ? "{}" : NameStr);
            }
            set
            {
                NameStr = value.ToString();
            }
        }
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


    }
}