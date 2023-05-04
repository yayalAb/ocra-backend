using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class MarriageApplication : BaseAuditableEntity
    {
        public DateTime ApplicationDate { get; set; }
        public Guid ApplicationAddressId { get; set; }
        public Guid BrideInfoId { get; set; }
        public Guid GroomInfoId { get; set;}
        public Guid CivilRegOfficerId { get; set; }

        public virtual Address ApplicationAddress { get; set;}
        public virtual PersonalInfo BrideInfo { get; set;}
        public virtual PersonalInfo GroomInfo { get; set;}
        public virtual PersonalInfo CivilRegOfficer { get; set;}
        public virtual MarriageEvent MarriageEvent { get; set; }

    }
}