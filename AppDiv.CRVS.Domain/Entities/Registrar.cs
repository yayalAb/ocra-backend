using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Registrar : BaseAuditableEntity
    {
        public Guid RelationshipId { get; set; }
        public Guid RegistrarInfoId { get; set; }
        public Guid EventId { get; set; }
        public virtual Lookup Relationship { get; set; }
        public virtual PersonalInfo RegistrarInfo { get; set; }
        public virtual Event Event { get; set; }
    }
}