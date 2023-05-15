using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class MarriageCertificateDTO
    {
        public JObject? BrideFirstName { get; set; }
        public JObject? BrideMiddleName { get; set; }
        public JObject? BrideLastName { get; set; }
        public LookupDTO? BrideNationality { get; set; }
        public JObject? GroomFirstName { get; set; }
        public JObject? GroomMiddleName { get; set; }
        public JObject? GroomLastName { get; set; }
        public LookupDTO? GroomNationality { get; set; }
        public DateTime? MarriageDate { get; set; }
        public AddressDTO? MarriagePlace { get; set; }
        public DateTime? EventRegDate { get; set; }
        public DateTime? CertificateGenerationDate { get; set; } = DateTime.Now;
        public JObject? CivilRegOfficerFirstName { get; set; }
        public JObject? CivilRegOfficerMiddleName { get; set; }
        public JObject? CivilRegOfficerLastName { get; set; }
    }
}