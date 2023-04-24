using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddSettingRequest
    {
        public string Key { get; set; }
        public JObject Value { get; set; }
    }
}


