
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CertificateSerialTransfer : BaseAuditableEntity
    {
        public string? SenderId {get; set; }
        public string RecieverId { get; set; }
        public bool Status { get; set; }
        public int  From { get; set; }
        public int  To  { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Reciever { get; set; }

    }
}