using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddGroupRequest
    {
        public string GroupName { get; set; }
        public JObject Description { get; set; }
        public JArray Roles { get; set; }
        public JArray ManagedGroups { get; set; }
        public bool ManageAll { get; set; } = false;

    }
}