


namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class CertificateIndex
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid? NestedEventId { get; set; }
        public string? EventType { get; set; }
        public string? CertificateId { get; set; }
        public string? CertificateSerialNumber { get; set; }
        public string? ContentStr { get; set; }
        public string? FullNameAm { get; set; }
        public string? FullNameOr { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? GenderOr { get; set; }
        public string? GenderAm { get; set; }
        public string? AddressOr { get; set; }
        public string? AddressAm { get; set; }
        public string? MotherFullNameAm { get; set; }
        public string? MotherFullNameOr { get; set; }
        public string? CivilRegOfficerNameAm { get; set; }
        public string? CivilRegOfficerNameOr { get; set; }
        public string? EventAddressAm { get; set; }
        public string? EventAddressOr { get; set; }
        public string? EventRegisteredAddressId { get; set; }
        public string? EventRegisteredAddressAm {get; set; }
        public string? EventRegisteredAddressOr {get; set; }
        public bool? Status { get; set; }







    }
}