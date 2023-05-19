using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class BirthEvent : BaseAuditableEntity
    {
        public Guid FatherId { get; set; }
        public Guid MotherId { get; set; }
        public Guid FacilityTypeLookupId { get; set; }
        public Guid FacilityLookupId { get; set; }
        public Guid BirthPlaceId { get; set; }
        public Guid TypeOfBirthLookupId { get; set; }
        public Guid EventId { get; set; }
        public virtual PersonalInfo Father { get; set; }
        public virtual PersonalInfo Mother { get; set; }
        public virtual Lookup FacilityTypeLookup { get; set; }
        public virtual Lookup FacilityLookup { get; set; }
        public virtual Lookup TypeOfBirthLookup { get; set; }
        public virtual Lookup BirthPlace { get; set; }
        public virtual Event Event { get; set; }
        public virtual BirthNotification BirthNotification { get; set; }

    }
}