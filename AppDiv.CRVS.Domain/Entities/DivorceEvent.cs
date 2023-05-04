namespace AppDiv.CRVS.Domain.Entities
{
    public class DivorceEvent
    {
        public Guid DivorcedWifeId { get; set; }
        public DateTime DataOfMarriage { get; set; }
        public DateTime DivorceDate { get; set; }
        public string DivorceReason { get; set; }
        public Guid CourtCaseId { get; set; }
        public int NumberChildren { get; set; }
        public Guid EventId { get; set; }

        public virtual PersonalInfo DivorcedWife { get; set; }
        public virtual CourtCase CourtCase { get; set; }
        public virtual Event Event { get; set; }
    }
}