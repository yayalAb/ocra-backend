using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class AdoptionCertificateDTO
    {
        public string? BirthCertificateNo { get; set; }
        public JObject? ChildFirstName { get; set; }
        public JObject? ChildMiddleName { get; set; }
        public JObject? ChildLastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public AddressDTO? BirthPlace { get; set; }
        public LookupDTO? ChildNationality { get; set; }
        public JObject? MotherFirstName { get; set; }
        public JObject? MotherMiddleName { get; set; }
        public JObject? MotherLastName { get; set; }
        public LookupDTO? MotherNationality { get; set; }
        public JObject? FatherFirstName { get; set; }
        public JObject? FatherMiddleName { get; set; }
        public JObject? FatherLastName { get; set; }
        public LookupDTO? FatherNationality { get; set; }
        public DateTime? EventRegDate { get; set; }
        public DateTime? CertificateGenerationDate { get; set; } = DateTime.Now;
        public JObject? CivilRegOfficerFirstName { get; set; }
        public JObject? CivilRegOfficerMiddleName { get; set; }
        public JObject? CivilRegOfficerLastName { get; set; }
    }
}