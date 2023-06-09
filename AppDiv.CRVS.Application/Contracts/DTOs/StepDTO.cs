using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class StepDTO
    {
        public Guid Id { get; set; }
        public int step { get; set; }
        public bool Status { get; set; }
        public JObject Description { get; set; }
        public Guid UserGroupId { get; set; }
    }
}