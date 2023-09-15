
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Notification : BaseAuditableEntity
    {
        public string Type { get; set; }
        public string MessageStr { get; set; }
        public Guid NotificationObjId {get; set; }
        public bool Seen { get; set; }= false;
        public Guid? RequestId {get; set;}
        public Guid? GroupId { get; set; }
        public Guid? ReceiverId {get;set;}
        public string SenderId { get; set; }
        public string ApprovalType {get;set;}
        public Guid? EventRegisteredAddressId {get;set;}

        public virtual UserGroup UserGroup { get; set; }
        public virtual Request Request { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual Address EventRegisteredAddress {get; set; }

    }
}