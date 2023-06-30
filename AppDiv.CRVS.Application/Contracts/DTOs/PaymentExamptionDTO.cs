using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentExamptionDTO
    {
        public Guid Id { get; set; }
        public Guid ExamptionReasonLookupId { get; set; }

        // public virtual PaymentExamptionRequestDTO ExamptionRequest { get; set; }
        public virtual ICollection<SupportingDocumentDTO> SupportingDocuments { get; set; }
    }
}