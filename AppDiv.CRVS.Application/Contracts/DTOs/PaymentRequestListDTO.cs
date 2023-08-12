using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentRequestListDTO
    {
        public Guid Id { get; set; }
        public string? RequestType { get; set; }

        public string? RequestedBy { get; set; }
        public string? RequestedDate { get; set; }
        public float? Amount { get; set; }
    }
}