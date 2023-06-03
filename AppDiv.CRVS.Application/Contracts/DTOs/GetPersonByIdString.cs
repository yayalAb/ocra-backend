using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class GetPersonByIdString
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? BirthDateEt { get; set; }
        public string? National { get; set; }
        public string Sex { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Title { get; set; }
        public string? Religion { get; set; }
        public string? EducationalStatus { get; set; }
        public string? TypeOfWork { get; set; }
        public string? MarriageStatus { get; set; }
        public string? Nation { get; set; }
        public string? BirthAddressOr { get; set; }
        public string? ResidentAddressOr { get; set; }
        public string? BirthAddressAm { get; set; }
        public string? ResidentAddressAm { get; set; }

    }
}