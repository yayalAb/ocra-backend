using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class UpdateCertificateTemplateRequest
    {
        public Guid Id { get; set; }
        public string CertificateType { get; set; }
        public IFormFile SvgFile { get; set; }

    }
}