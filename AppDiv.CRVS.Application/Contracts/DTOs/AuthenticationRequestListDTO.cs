using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AuthenticationRequestListDTO
    {
        public Guid Id { get; set; }
        public string? RequestedBy { get; set; }
        public string? CertificateId { get; set; }
        public string? RequestType { get; set; }
        public string? CertificateType { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? CurrentStep { get; set; }
    }
}