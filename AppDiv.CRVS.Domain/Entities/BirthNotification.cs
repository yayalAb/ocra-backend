using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class BirthNotification : BaseAuditableEntity
    {
        public Guid? BirthEventId { get; set; }
        public Guid DeliveryTypeLookupId { get; set; }
        public float WeightAtBirth { get; set; }
        public Guid SkilledProfLookupId { get; set; }
        public string NotficationSerialNumber { get; set; }
        public virtual BirthEvent BirthEvent { get; set; }
        public virtual Lookup DeliveryTypeLookup { get; set; }
        public virtual Lookup SkilledProfLookup { get; set; }

    }
}