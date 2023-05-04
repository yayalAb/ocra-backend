using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class BirthEvent : BaseAuditableEntity
    {
        public Guid FatherId { get; set; }
        public Guid MotherId { get; set; }
        public Guid FacilityTypeId { get; set; }
        public Guid FacilityId { get; set; }
        public Guid BirthPlaceId { get; set; }
        public Guid TypeOfBirthId { get; set; }
        public Guid EventId { get; set; }
        public virtual PersonalInfo Father { get; set; }
        public virtual PersonalInfo Mother { get; set; }
        public virtual Lookup FacilityType { get; set; }
        public virtual Lookup Facility { get; set; }
        public virtual Lookup TypeOfBirth { get; set; }
        public virtual Address BirthPlace { get; set; }
        public virtual Event Event { get; set; }
        public virtual BirthNotification BirthNotification { get; set; }

    }
}