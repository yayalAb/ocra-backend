using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddPaymentExamptionRequest
    {
        public Guid ExamptionRequestId { get; set; }
        public string Document { get; set; }
    }
}