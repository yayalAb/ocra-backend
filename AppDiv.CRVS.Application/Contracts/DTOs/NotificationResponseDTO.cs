

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class NotificationResponseDTO
    {
        public Guid Id {get; set; }
        public string Type { get; set; }
        public string MessageStr { get; set; }
        public Guid NotificationObjId {get; set; }
        // public bool Seen { get; set; }
        public Guid? RequestId {get; set;}
        public Guid GroupId { get; set; }
    }
}