using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CertificateDTO
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public string ContentStr { get; set; }
        public bool Status { get; set; }
        public bool AuthenticationStatus { get; set; }
        public int PrintCont { get; set; }
        public string CertificateSerialNumber { get; set; }

        public EventDTO Event { get; set; }
    }
}