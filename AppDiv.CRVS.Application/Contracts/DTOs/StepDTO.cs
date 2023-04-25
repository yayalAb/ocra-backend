using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class StepDTO
    {
        public int step { get; set; }
        public string ResponsibleGroup { get; set; }
        public float Payment { get; set; }
        public float Status { get; set; }
        public JObject Descreption { get; set; }
        public Guid workflowId { get; set; }
    }
}