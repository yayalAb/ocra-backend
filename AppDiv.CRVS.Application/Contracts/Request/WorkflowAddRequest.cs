using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class WorkflowAddRequest
    {
        public string workflowName { get; set; }
        public decimal Payment { get; set; } = 0;
        public int? PaymentStep { get; set; } = 0;
        public JObject? Description { get; set; }
        public ICollection<StepDTO> Steps { get; set; }
    }
}