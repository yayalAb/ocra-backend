using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Registrar : BaseAuditableEntity
    {
        public Guid? RelationshipLookupId { get; set; }
        public Guid RegistrarInfoId { get; set; }
        public Guid EventId { get; set; }
        public virtual Lookup RelationshipLookup { get; set; }
        public virtual PersonalInfo RegistrarInfo { get; set; }
        public virtual Event Event { get; set; }
    }
}