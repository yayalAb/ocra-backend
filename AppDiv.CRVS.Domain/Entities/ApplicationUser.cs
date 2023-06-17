
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AppDiv.CRVS.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Otp { get; set; }
        public DateTime? OtpExpiredDate { get; set; }
        public Guid PersonalInfoId { get; set; }
        public Guid AddressId { get; set; }
        public virtual PersonalInfo PersonalInfo { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<CertificateSerialTransfer> SenderCertificateSerialTransfers { get; set; }
        public virtual ICollection<CertificateSerialTransfer> RecieverCertificateSerialTransfers { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Transaction> CivilRegOfficerTransactions { get; set; }


    }
}
