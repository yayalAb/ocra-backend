using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class BirthNotification : BaseAuditableEntity
    {
        public Guid BirthEventId { get; set; }

        public Guid DeliveryTypeId { get; set; }
        public float WeghtAtBirth { get; set; }
        public Guid SkilledProfId { get; set; }
        public string NotficationSerialNumber { get; set; }
        public virtual BirthEvent BirthEvent { get; set; }
        public virtual Lookup DeliveryType { get; set; }
        public virtual Lookup SkilledProf { get; set; }

    }
}