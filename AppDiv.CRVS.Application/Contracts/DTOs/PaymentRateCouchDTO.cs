using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentRateCouchDTO
    {
        public Guid Id { get; set; }
        public LookupCouchDTO PaymentTypeLookup { get; set; }
        public LookupCouchDTO EventLookup { get; set; }
        public AddressCouchDTO Address { get; set; }
        public float Amount { get; set; }
        public float Backlog { get; set; }
        public float HasCamera { get; set; }
        public float HasVideo { get; set; }
        public bool Status { get; set; }
        public bool IsForeign { get; set; } 
    }
}