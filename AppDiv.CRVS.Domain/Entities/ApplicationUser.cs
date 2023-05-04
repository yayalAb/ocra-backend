
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AppDiv.CRVS.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Otp { get; set; }
        public DateTime? OtpExpiredDate { get; set; }
        public Guid PersonalInfoId { get; set; }
        public virtual PersonalInfo PersonalInfo { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }

    }
}
