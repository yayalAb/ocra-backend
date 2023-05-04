using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class DeathEvent  : BaseAuditableEntity
    {
        public Guid FacilityTypeId { get; set; }
        public Guid FacilityId { get; set; }
        public string DuringDeath { get; set; }
        public string PlaceOfFuneral { get; set; }
        public Guid EventId { get; set; }

        public virtual Lookup FacilityType { get; set; }
        public virtual Lookup Facility { get; set; }
        public virtual Event Event { get; set; }
        public virtual DeathNotification DeathNotification { get; set; }

    }
}