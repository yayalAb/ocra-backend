using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class GetAllWorkFlowDTO
    {
        public Guid id { get; set; }
        public string workflowName { get; set; }
        public string? ResiponsbleGroup { get; set; }
        public int step { get; set; }
        public bool HasPayment { get; set; } = false;
        public int? PaymentStep { get; set; }
        public bool status { get; set; }

    }
}