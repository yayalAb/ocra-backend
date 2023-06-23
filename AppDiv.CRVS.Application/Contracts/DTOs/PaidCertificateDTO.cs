

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaidCertificateDTO
    {
        public Guid EventId { get; set; }
        public string CertificateId { get; set; }
        public string EventType { get; set; }
        public string OwnerFullName { get; set; }
        public String? EventDate { get; set; }
        public String? EventRegDate { get; set; }
        public bool IsCertified { get; set; } = false;

    }
}