using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class MarriageCertificateResponseDTO
    {
        public string BrideImage { get; set; }
        public string GroomImage { get; set; }
        public JObject Content { get; set; }
        public Guid? TemplateId { get; set; }
    }
}