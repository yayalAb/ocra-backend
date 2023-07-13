using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class DeathEvent : BaseAuditableEntity
    {
        public string? BirthCertificateId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? DuringDeathId { get; set; }
        public Guid? DeathPlaceId { get; set; }
        public string? PlaceOfFuneral { get; set; }
        public Guid EventId { get; set; }

        public virtual Lookup FacilityTypeLookup { get; set; }
        public virtual Lookup FacilityLookup { get; set; }
        public virtual Event Event { get; set; }
        public virtual Lookup DuringDeathLookup { get; set; }
        public virtual Lookup DeathPlace { get; set; }
        public virtual DeathNotification DeathNotification { get; set; }

    }
}