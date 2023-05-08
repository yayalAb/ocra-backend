using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class ContactInfo : BaseAuditableEntity
    {
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? HouseNumber { get; set; }
        public string? Website { get; set; }
        public string? Linkdin { get; set; }
        // public Guid PersonalInfoId  { get; set;}

        // public virtual PersonalInfo PersonalInfo { get; set; }


    }
}
