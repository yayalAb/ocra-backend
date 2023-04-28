using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class StepDTO
    {
        public int step { get; set; }
        public decimal Payment { get; set; }
        public bool Status { get; set; }
        public JObject Descreption { get; set; }
        public List<Guid> UserGroups { get; set; }
        // public Guid workflowId { get; set; }
    }
}