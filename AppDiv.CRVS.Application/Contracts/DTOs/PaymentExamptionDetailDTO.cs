using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentExamptionDetailDTO
    {
        public Guid Id { get; set; }
        public string RequestedBy { get; set; }
        public string? NationalId { get; set; }
        public string? Fullname { get; set; }
        public string? ExamptedDate { get; set; }
        public string? CertificateType { get; set; }
        public string? ReasonOr { get; set; }
        public string? ReasonAm { get; set; }
        public int? NumberOfClient { get; set; }
        public string? RequestedAddress { get; set; }
        public bool Status { get; set; }
        public Guid? RequestId { get; set; }

    }
}