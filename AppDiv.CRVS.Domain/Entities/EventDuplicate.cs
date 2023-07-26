

namespace AppDiv.CRVS.Domain.Entities
{
    public class EventDuplicate
    {
        public Guid OldEventId { get; set; }
        public Guid NewEventId { get; set; }
        public string FoundWith { get; set; }
        public string Status { get; set; }
        public string? CorrectedBy {get; set;}
        public virtual Event OldEvent { get; set; }
        public virtual Event NewEvent { get; set; }

    }
}