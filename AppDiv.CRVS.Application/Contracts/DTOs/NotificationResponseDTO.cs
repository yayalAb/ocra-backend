
namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class NotificationResponseDTO
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string MessageStr { get; set; }
        public Guid NotificationObjId { get; set; }
        public Guid? EventId { get; set; }
        public string SenderId { get; set; }
        // public bool Seen { get; set; }
        public Guid? RequestId { get; set; }
        public Guid? GroupId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SenderUserName { get; set; }
        public string SenderFullName { get; set; }
        public string ApprovalType { get; set; }

        public bool hasApproval { get; set; }

    }
}