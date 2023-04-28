using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class StepDTO
    {
        public Guid Id { get; set; }
        public int step { get; set; }
        public decimal Payment { get; set; }
        public bool Status { get; set; }
        public JObject Descreption { get; set; }
        public Guid UserGroupId { get; set; }
    }
}