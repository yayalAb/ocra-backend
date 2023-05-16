using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class DivorceCertificateDTO
    {
        public JObject? WifeFirstName { get; set; }
        public JObject? WifeMiddleName { get; set; }
        public JObject? WifeLastName { get; set; }
        public LookupDTO? WifeNationality { get; set; }
        public JObject? HusbandFirstName { get; set; }
        public JObject? HusbandMiddleName { get; set; }
        public JObject? HusbandLastName { get; set; }
        public LookupDTO? HusbandNationality { get; set; }
        public DateTime? DivorceDate { get; set; }
        public AddressDTO? DivorcePlace { get; set; }
        public DateTime? EventRegDate { get; set; }
        public DateTime? CertificateGenerationDate { get; set; } = DateTime.Now;
        public JObject? CivilRegOfficerFirstName { get; set; }
        public JObject? CivilRegOfficerMiddleName { get; set; }
        public JObject? CivilRegOfficerLastName { get; set; }
    }
}