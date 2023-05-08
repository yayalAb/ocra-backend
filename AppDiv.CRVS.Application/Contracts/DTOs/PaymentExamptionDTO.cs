using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentExamptionDTO
    {
        public Guid ExamptionRequestId { get; set; }
        public string Document { get; set; }
    }
}