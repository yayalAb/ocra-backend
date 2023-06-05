using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CertificateDTO
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public JObject Content { get; set; }
        public bool Status { get; set; }
        public bool AuthenticationStatus { get; set; }
        public int PrintCount { get; set; }
        public string CertificateSerialNumber { get; set; }
        public EventDTO Event { get; set; }
    }
}