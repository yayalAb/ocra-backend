using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class BaseArchiveDTO
    {
        public IList<SupportingDocumentDTO>? EventSupportingDocuments { get; set; }
        public IList<SupportingDocumentDTO>? PaymentExamptionSupportingDocuments { get; set; }
    }
}