
namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class CertificateIndex
    {
        public Guid Id { get; set; }
        public string? EventType {get; set; }
        public string? CertificateId {get; set; }
        public string? CertificateSerialNumber {get; set; }
        public string? ContentStr { get; set; }
        public string? FirstNameOr { get; set; }
        public string? FirstNameAm { get; set; }
        public string? MiddleNameOr { get; set; }
        public string? MiddleNameAm { get; set; }
        public string? LastNameOr { get; set; }
        public string? LastNameAm { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? BirthDate {get; set;}
        public string? GenderOr { get; set; }
        public string? GenderAm { get; set; }
        public string? AddressOr { get;set; }
        public string? AddressAm { get; set;}

        
        
    


    }
}