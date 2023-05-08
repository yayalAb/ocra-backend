using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentExamptionRequestDTO
    {
        public Guid Id { get; set; }
        public string ReasonStr { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullNAme { get; set; }
        public DateTime ExamptedDate { get; set; }
        public string ExamptedBy { get; set; }
        public string? NumberOfClient { get; set; }
    }
}