
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Setting : BaseAuditableEntity
    {
        public string Key { get; set; }
        public  string ValueStr { get; set; }
        [NotMapped]
        public JObject Value
        {

            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ValueStr) ? "{}" : ValueStr);
            }
            set
            {
                ValueStr = value.ToString();
            }
        }
    }
}