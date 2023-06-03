
namespace AppDiv.CRVS.Application.Contracts.DTOs.ElasticSearchDTOs
{
    public class PersonalInfoIndex
    {
        public Guid Id { get; set; }
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
    


    }
}