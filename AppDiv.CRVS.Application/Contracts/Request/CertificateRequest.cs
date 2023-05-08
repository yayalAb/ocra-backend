using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class CertificateRequest
    {
        public Guid EventId { get; set; }
        public JObject Content { get; set; }
        public bool Status { get; set; }
        public bool AuthenticationStatus { get; set; }
        public int PrintCont { get; set; }
        public string CertificateSerialNumber { get; set; }
    }
}