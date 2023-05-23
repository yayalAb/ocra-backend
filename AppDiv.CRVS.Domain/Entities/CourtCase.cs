using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CourtCase : BaseAuditableEntity
    {
        public Guid CourtId { get; set; }
        public string CourtCaseNumber { get; set; }
        public DateTime ConfirmedDate { get; set; }
        public string? ConfirmedDateEt { get; set; }

        public virtual Court Court { get; set; }
        public virtual AdoptionEvent AdoptionEventCourtCase { get; set; }

        public virtual DivorceEvent DivorceEventCourtCase { get; set; }
    }
}