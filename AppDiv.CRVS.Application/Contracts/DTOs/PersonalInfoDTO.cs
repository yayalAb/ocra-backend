using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PersonalInfoDTO
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? NationalId { get; set; }
        public string? PlaceOfBirthLookup { get; set; }
        public string? NationalityLookup { get; set; }
        public string? TitleLookup { get; set; }
        public string? ReligionLookup { get; set; }
        public string? EducationalStatusLookup { get; set; }
        public string? TypeOfWorkLookup { get; set; }
        public string? MarraigeStatusLookup { get; set; }
        // public AddressDTO? Address { get; set; }
        public string? NationLookup { get; set; }
        public DateTime? CreatedDate { get; set; }
        // public string ContactInfo { get; set; }
        public ContactInfoDTO? ContactInfo { get; set; }

    }
}