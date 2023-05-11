using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentExamptionRequestDTO
    {
        public Guid Id { get; set; }
        public string ReasonStr { get; set; }
        public JObject Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        public DateTime ExamptedDate { get; set; }
        public string ExamptedBy { get; set; }
        public string? NumberOfClient { get; set; }
    }
}