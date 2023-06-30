using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentRateDTO
    {
        public Guid Id { get; set; }
        public LookupDTO PaymentTypeLookup { get; set; }
        public LookupDTO EventLookup { get; set; }
        public Address Address { get; set; }
        public float Amount { get; set; }
        public float Backlog { get; set; }
        public float HasCamera { get; set; }
        public float HasVideo { get; set; }
        public bool Status { get; set; }
    }
}