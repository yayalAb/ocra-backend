
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CertificateSerialRange : BaseAuditableEntity
    {
        public int  From { get; set; }
        public int  To  { get; set; }
        public Guid AddressId { get; set; }
        public virtual Address Address { get; set; }
        

    }
}