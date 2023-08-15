
namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class CertificateIndex 
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid? NestedEventId { get; set; }
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
        public string? MotherFirstNameAm {get; set; }
        public string? MotherFirstNameOr {get; set; }
        public string? MotherMiddleNameAm {get; set; }
        public string? MotherMiddleNameOr {get; set; }
        public string? MotherLastNameAm {get; set; }
        public string? MotherLastNameOr {get; set; }
        public string? CivilRegOfficerNameAm {get; set; }
        public string? CivilRegOfficerNameOr {get; set; }
        public string? EventAddressAm {get; set; }
        public string? EventAddressOr {get; set; }
        public Guid? EventRegisteredAddressId {get; set; }


        
        
    


    }
}