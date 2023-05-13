using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AdoptionPersonalINformationRequest
    {
        public class AddAdoptionPersonalInfoRequest
        {
            public Guid Id { get; set; }
            public LanguageModel FirstName { get; set; }
            public LanguageModel? MiddleName { get; set; }
            public LanguageModel? LastName { get; set; }
            public DateTime? BirthDate { get; set; }
            public string? NationalId { get; set; }
            public Guid SexLookupId { get; set; }
            public Guid? PlaceOfBirthLookupId { get; set; }
            public Guid NationalityLookupId { get; set; }
            public Guid? ReligionLookupId { get; set; }
            public Guid? EducationalStatusLookupId { get; set; }
            public Guid? TypeOfWorkLookupId { get; set; }
            public Guid? MarriageStatusLookupId { get; set; }
            public Guid? NationLookupId { get; set; }
        }
    }
}