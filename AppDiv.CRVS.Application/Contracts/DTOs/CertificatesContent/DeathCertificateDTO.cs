using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class DeathCertificateDTO
    {
        public JObject? FirstName { get; set; }
        public JObject? MiddleName { get; set; }
        public JObject? LastName { get; set; }
        public LookupDTO? Title { get; set; }
        public LookupDTO? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public AddressDTO? DeathPlace { get; set; }
        public DateTime? DeathDate { get; set; }
        public LookupDTO? Nationality { get; set; }
        public DateTime? EventRegDate { get; set; }
        public DateTime? CertificateGenerationDate { get; set; } = DateTime.Now;
        public JObject? CivilRegOfficerFirstName { get; set; }
        public JObject? CivilRegOfficerMiddleName { get; set; }
        public JObject? CivilRegOfficerLastName { get; set; }
    }
}