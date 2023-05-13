

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class UnPaidCertificateDTO
    {
        public Guid EventId { get; set; }
        public string CertificateId { get; set; }
        public string EventType { get; set; }
        public string OwnerFullName { get; set; }
        public Guid PaymentRequestId { get; set; }
        public float? Amount { get; set; }


    }
}