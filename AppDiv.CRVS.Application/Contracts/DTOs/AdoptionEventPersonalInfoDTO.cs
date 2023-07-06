using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AdoptionEventPersonalInfoDTO
    {
        public Guid Id { get; set; }
        public JObject FirstName { get; set; }
        public JObject? MiddleName { get; set; }
        public JObject? LastName { get; set; }
        public string BirthDateEt { get; set; }
        public string? NationalId { get; set; }
        public Guid? SexLookupId { get; set; }
        public Guid? BirthAddressId { get; set; }
        public Guid? ResidentAddressId { get; set; }
        public Guid NationalityLookupId { get; set; }
        public Guid? ReligionLookupId { get; set; }
        public Guid? EducationalStatusLookupId { get; set; }
        public Guid? TypeOfWorkLookupId { get; set; }
        public Guid? MarriageStatusLookupId { get; set; }
        public Guid? NationLookupId { get; set; }
        public string? PhoneNumber { get; set; }
        public AddressResponseDTOE? BirthAddressResponsDTO { get; set; }
        public AddressResponseDTOE? ResidentAddressResponsDTO { get; set; }

    }
}