using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Utility.Services;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Event : BaseAuditableEntity
    {
        Event() : base()
        {
            var httpContext = new HttpContextAccessor().HttpContext;
            var tokenstring = httpContext?.Request.Headers["Authorization"].ToString().Split(" ").Last();
            if (!string.IsNullOrEmpty(tokenstring))
            {
                var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
                EventRegisteredAddressId = new Guid(token.Claims.FirstOrDefault(c => c.Type == "addressId")?.Value);
            }
        }

        public string EventType { get; set; }
        public string? RegBookNo { get; set; }
        public string? CivilRegOfficeCode { get; set; }
        public string? CertificateId { get; set; }
        public Guid EventOwenerId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventRegDate { get; set; }

        public string? EventDateEt { get; set; }
        public string EventRegDateEt { get; set; }

        public Guid? EventAddressId { get; set; }
        public Guid? EventRegisteredAddressId { get; set; }
        public string? InformantType { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public bool IsPaid { get; set; } = false;
        public bool IsCertified { get; set; } = false;
        public bool? IsOfflineReg { get; set; } = false;
        public bool IsVerified { get; set; } = false;
        public bool HasPendingDocumentApproval { get; set; } = false;
        public bool OnReprintPaymentRequest { get; set; } = false;
        public bool ReprintWaiting { get; set; } = false;
        public virtual Address EventAddress { get; set; }
        public virtual Address EventRegisteredAddress { get; set; }

        public virtual PersonalInfo EventOwener { get; set; }
        public virtual PersonalInfo CivilRegOfficer { get; set; }
        public virtual BirthEvent BirthEvent { get; set; }
        public virtual Registrar? EventRegistrar { get; set; }
        public virtual DeathEvent DeathEventNavigation { get; set; }
        public virtual AdoptionEvent AdoptionEvent { get; set; }
        public virtual DivorceEvent DivorceEvent { get; set; }
        public virtual PaymentExamption? PaymentExamption { get; set; }
        public virtual VerficationRequest VerficationRequestNavigation { get; set; }
        public virtual ICollection<PaymentRequest> EventPaymentRequest { get; set; }
        public virtual ICollection<Certificate> EventCertificates { get; set; }

        public virtual MarriageEvent MarriageEvent { get; set; }
        public virtual ICollection<SupportingDocument> EventSupportingDocuments { get; set; }
        public virtual ICollection<CorrectionRequest> CorrectionRequests
        {
            get; set;
        }
        public virtual ICollection<EventDuplicate> NewEventDuplicatesNavigation { get; set; }
        public virtual ICollection<EventDuplicate> OldEventDuplicatesNavigation { get; set; }

        [NotMapped]
        public string? _EventDateEt
        {
            get { return EventDateEt; }
            set
            {
                EventDateEt = value;

                EventDate = new CustomDateConverter(EventDateEt).gorgorianDate;
            }
        }
        [NotMapped]
        public string? _EventRegDateEt
        {
            get { return _EventRegDateEt; }
            set
            {
                EventRegDateEt = value;
                EventRegDate = new CustomDateConverter(EventRegDateEt).gorgorianDate;
            }
        }

    }
}