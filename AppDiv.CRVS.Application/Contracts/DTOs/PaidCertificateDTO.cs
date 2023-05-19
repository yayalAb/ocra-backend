

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaidCertificateDTO
    {
        public Guid EventId { get; set; }
        public string CertificateId { get; set; }
        public string EventType { get; set; }
        public string OwnerFullName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventRegDate { get; set; }
        public bool IsCertified { get; set; } = false;

    }
}