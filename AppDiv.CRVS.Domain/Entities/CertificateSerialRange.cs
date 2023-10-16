
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CertificateSerialRange : BaseAuditableEntity
    {
        public string From { get; set; }
        public string To  { get; set; }
        public Guid AddressId { get; set; }
        public bool? IsDamaged { get; set; }
        public string? UserId { get; set; }
        public ApplicationUser User { get; set; }
        public virtual Address Address { get; set; }
        

    }
}