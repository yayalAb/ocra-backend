using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class GetAllWorkFlowDTO
    {
        public Guid id { get; set; }
        public string workflowName { get; set; }
        public int step { get; set; }
        public string responsibleGroup { get; set; }
        public float payment { get; set; }
        public float status { get; set; }

    }
}