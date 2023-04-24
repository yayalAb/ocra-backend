using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class LookupDTO
    {
        public string id { get; set; }
        public string Key { get; set; }
        public JObject Value { get; set; }
        public JObject Description { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
    }
}

