
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AppDiv.CRVS.Domain
{
    public class ApplicationUser : IdentityUser
    {

        public string? Otp { get; set; }
        public bool Status { get; set; } = true;
        public DateTime? OtpExpiredDate { get; set; }
        public Guid PersonalInfoId { get; set; }
        public Guid AddressId { get; set; }
        public string PreferedLanguage { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public virtual Guid? CreatedBy { get; set; }
        public virtual Guid? ModifiedBy { get; set; }
        public string? PasswordResetOtp { get; set; }
        public DateTime? PasswordResetOtpExpiredDate { get; set; }
        public int SelectedAdminType { get; set; } = 0;
        public bool? CanRegisterEvent {get;set;} = null;

        public virtual PersonalInfo PersonalInfo { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        public virtual ICollection<CertificateSerialTransfer> SenderCertificateSerialTransfers { get; set; }
        public virtual ICollection<CertificateSerialTransfer> RecieverCertificateSerialTransfers { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Transaction> CivilRegOfficerTransactions { get; set; }
        public virtual ICollection<LoginHistory> LoginHistorys { get; set; }
        // public virtual ICollection<WorkHistory> WorkerHistories { get; set; }

        public virtual ICollection<Message> SentMessages {get; set; }
        public virtual ICollection<Message> ReceivedMessages {get; set; }

    }
}
