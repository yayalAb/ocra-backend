
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;


namespace AppDiv.CRVS.Domain.Entities
{
    public class Message : BaseAuditableEntity
    {
        public string Type { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId {get; set; }
        public string? TextMessage {get; set; }
        public Guid? ParentMessageId { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Receiver { get; set; }
        public virtual Message ParentMessage {get; set; }
        public virtual ICollection<Message> ChildMessages { get; set; } 
    }
}