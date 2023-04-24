using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class GroupDTO
    {
        public Guid id { get; set; }
        public string GroupName { get; set; }
        public JObject Description { get; set; }
        public JArray Roles { get; set; }
    }
}