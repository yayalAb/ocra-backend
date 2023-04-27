using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PaymentRate : BaseAuditableEntity
    {
        public Guid PaymentTypeLookupId { get; set; }
        public Guid EventLookupId { get; set; }
        public Guid AddressId { get; set; }
        public float Amount { get; set; }
        public bool Status { get; set; }

        public virtual Lookup PaymentTypeLookup { get; set; }
        public virtual Lookup EventLookup { get; set; }
        public virtual Address Address { get; set; }

    }
}