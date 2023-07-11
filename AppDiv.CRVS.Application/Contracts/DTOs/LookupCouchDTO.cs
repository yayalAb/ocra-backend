using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;


namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class LookupCouchDTO
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string ValueStr { get; set; }

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
        // public JObject? Description { get; set; }
        public string? StatisticCode { get; set; }
        public string? Code { get; set; }
    }
}

