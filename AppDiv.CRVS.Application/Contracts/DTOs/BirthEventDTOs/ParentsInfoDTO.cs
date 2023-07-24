using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PersonInfoDTO
    {
        public Guid? Id { get; set; } = null;
        public LanguageModel FirstName { get; set; }
        public LanguageModel MiddleName { get; set; }
        public LanguageModel LastName { get; set; }
        // public DateTime? BirthDate { get; set; }
        public string? NationalId { get; set; }
        // public Guid? PlaceOfBirthLookupId { get; set; }
        public Guid NationalityLookupId { get; set; }
        public Guid? ReligionLookupId { get; set; }
        public Guid? EducationalStatusLookupId { get; set; }
        public Guid? TypeOfWorkLookupId { get; set; }
        public Guid? MarriageStatusLookupId { get; set; }
        public Guid? ResidentAddressId { get; set; }
        public Guid? NationLookupId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }
    }
}