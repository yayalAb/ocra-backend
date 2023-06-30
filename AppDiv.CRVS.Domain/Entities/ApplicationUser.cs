
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AppDiv.CRVS.Domain
{
    public class ApplicationUser : IdentityUser
    {

        public string? Otp { get; set; }
        public bool Status {get; set; } = true;
        public DateTime? OtpExpiredDate { get; set; }
        public Guid PersonalInfoId { get; set; }
        public Guid AddressId { get; set; }
        public string PreferedLanguage { get;set; }

         public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public virtual Guid? CreatedBy { get; set; }
        public virtual Guid? ModifiedBy { get; set; }
        public bool ResetPasswordOtpOn {get; set ;} = false;
        
        public virtual PersonalInfo PersonalInfo { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<CertificateSerialTransfer> SenderCertificateSerialTransfers { get; set; }
        public virtual ICollection<CertificateSerialTransfer> RecieverCertificateSerialTransfers { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Transaction> CivilRegOfficerTransactions { get; set; }
        public virtual ICollection<LoginHistory> LoginHistorys { get; set; }



    }
}
