using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class UpdatePaymentExamptionRequest
    {
        public Guid Id { get; set;}
        public Guid ExamptionRequestId { get; set; }
        public ICollection<UpdateSupportingDocumentRequest> SupportingDocuments { get; set; }

    }
}