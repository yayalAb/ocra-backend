using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class PaymentExamptionRequestRequest
    {
        public LanguageModel Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        // public DateTime ExamptedDate { get; set; }
        public string ExamptedDateEt { get; set; }
        public string ExamptedBy { get; set; }
        public int? NumberOfClient { get; set; }
        public Guid? AddressId { get; set; }
        public string CertificateType { get; set; }

        public PaymentExamptionRequestRequest()
        {
            // this.ExamptedDate = DateTime.Now;
        }
    }
}