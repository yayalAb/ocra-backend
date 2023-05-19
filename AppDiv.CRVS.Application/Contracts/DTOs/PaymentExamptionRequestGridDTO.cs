namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PaymentExamptionRequestGridDTO
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        public DateTime ExamptedDate { get; set; }
        public string ExamptedBy { get; set; }
        public string? NumberOfClient { get; set; }
        public string CertificateType { get; set; }
        public AddressDTO? Address { get; set; }
    }
}