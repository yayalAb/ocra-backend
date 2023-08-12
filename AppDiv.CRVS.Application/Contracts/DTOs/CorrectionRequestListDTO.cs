using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CorrectionRequestListDTO
    {
        public Guid Id { get; set; }
        public string? Requestedby { get; set; }
        public string? EventType { get; set; }
        public string? RequestType { get; set; }
        public string? RequestDate { get; set; }
        public int? CurrentStatus { get; set; }
        public bool? CanEdit { get; set; }

    }
}