using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class MarriageEvent : BaseAuditableEntity
    {
        public Guid BrideInfoId {get; set; }
        public Guid MarriageTypeId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid EventId { get; set; }

        public virtual PersonalInfo BrideInfo { get; set; }
        public virtual Lookup MarriageType { get; set; }
        public virtual Event Event { get;set;}
        public virtual MarriageApplication Application {get; set; }
        public virtual ICollection<Witness> Witnesses { get; }
    }
}