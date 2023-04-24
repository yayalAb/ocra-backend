using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddLookupRequest
    {
        public string Key { get; set; }
        public JObject Value { get; set; }
        public JObject Description { get; set; }
        public string StatisticCode { get; set; }
        public string Code { get; set; }
    }
}

