using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class DeathCertificateDTO
    {
        public string? CertifcateId { get; set; }
        public string? RegBookNo { get; set; }
        public string? BirthCertifcateId { get; set; }
        // public JObject? FirstName { get; set; }
        public string? FirstNameAm { get; set; }
        public string? FirstNameOr { get; set; }

        // public JObject? MiddleName { get; set; }
        public string? MiddleNameAm { get; set; }
        public string? MiddleNameOr { get; set; }

        // public JObject? LastName { get; set; }
        public string? LastNameAm { get; set; }
        public string? LastNameOr { get; set; }
        public LookupDTO? Title { get; set; }

        // public LookupDTO? Gender { get; set; }
        public string? GenderAm { get; set; }
        public string? GenderOr { get; set; }

        // public DateTime? BirthDate { get; set; }
        public string? BirthMonth { get; set; }
        public string? BirthDay { get; set; }
        public string? BirthYear { get; set; }

        // public AddressDTO? DeathPlace { get; set; }
        public string? DeathPlaceOr { get; set; }
        public string? DeathPlaceAm { get; set; }

        // public DateTime? DeathDate { get; set; }
        public string? DeathMonth { get; set; }
        public string? DeathDay { get; set; }
        public string? DeathYear { get; set; }

        // public LookupDTO? Nationality { get; set; }
        public string? NationalityOr { get; set; }
        public string? NationalityAm { get; set; }

        // public DateTime? EventRegDate { get; set; }
        public string? EventRegisteredMonth { get; set; }
        public string? EventRegisteredDay { get; set; }
        public string? EventRegisteredYear { get; set; }

        // public DateTime? CertificateGenerationDate { get; set; } = DateTime.Now;
        public string? GeneratedMonth { get; set; }
        public string? GeneratedDay { get; set; }
        public string? GeneratedYear { get; set; }

        // public JObject? CivilRegOfficerFirstName { get; set; }
        // public JObject? CivilRegOfficerMiddleName { get; set; }
        // public JObject? CivilRegOfficerLastName { get; set; }
        public string? CivileRegOfficerFullNameOr { get; set; }
        public string? CivileRegOfficerFullNameAm { get; set; }
    }
}