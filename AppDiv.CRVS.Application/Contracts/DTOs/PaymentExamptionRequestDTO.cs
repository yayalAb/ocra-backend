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
        public JObject Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        // public DateTime ExamptedDate { get; set; }
        public string ExamptedDateEt { get; set; }
        public string ExamptedById { get; set; }
        public int? NumberOfClient { get; set; }
        public string CertificateType { get; set; }
        public AddressDTO? Address { get; set; }

    }
}