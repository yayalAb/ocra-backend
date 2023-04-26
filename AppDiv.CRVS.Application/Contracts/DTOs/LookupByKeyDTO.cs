using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class LookupByKeyDTO
    {
        public string id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

