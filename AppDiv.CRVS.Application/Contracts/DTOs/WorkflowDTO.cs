using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class WorkflowDTO
    {
        public Guid id { get; set; }
        public string workflowName { get; set; }
        public bool Status { get; set; }
        public bool HasPayment { get; set; } = false;
        public int? PaymentStep { get; set; } = 0;
        public JObject Description { get; set; }

        public ICollection<StepDTO> Steps { get; set; }
    }
}