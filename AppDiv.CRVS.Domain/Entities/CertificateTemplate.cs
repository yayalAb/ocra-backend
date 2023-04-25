
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CertificateTemplate : BaseAuditableEntity
    {
        public string CertificateType { get; set; }
        public string? fileUrl { get; set; }

    }
}