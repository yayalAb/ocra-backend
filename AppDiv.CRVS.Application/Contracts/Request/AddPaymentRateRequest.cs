using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddPaymentRateRequest
    {
        public Guid PaymentTypeLookupId { get; set; }
        public Guid EventLookupId { get; set; }
        public Guid AddressId { get; set; }
        public float Amount { get; set; }
        public bool Status { get; set; }
    }
}