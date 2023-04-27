using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FetchPaymentRateDTO
    {
        public Guid Id { get; set; }
        public string PaymentType { get; set; }
        public Guid PaymentTypeId { get; set; }
        public string Event { get; set; }
        public Guid EventId { get; set; }
        public string Address { get; set; }
        public Guid AddressId { get; set; }
        public float Amount { get; set; }
        public bool Status { get; set; }

    }
}