using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class BaseArchiveDTO
    {
        public ICollection<Guid>? EventSupportingDocuments { get; set; }
        public ICollection<Guid>? PaymentExamptionSupportingDocuments { get; set; }
    }
}