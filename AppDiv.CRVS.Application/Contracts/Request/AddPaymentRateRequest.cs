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
        public bool IsForeign { get; set; } = false;
        public float Amount { get; set; } = 0;
        public float Backlog { get; set; } = 0;
        public float HasCamera { get; set; } = 0;
        public float HasVideo { get; set; } = 0;

        public bool Status { get; set; }
    }
}