using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class DivorceCertificateDTO
    {
        public string? CertifcateId { get; set; }
        public string? RegBookNo { get; set; }
        public string? BirthCertifcateId { get; set; }
        // public JObject? WifeFirstName { get; set; }
        // public JObject? WifeMiddleName { get; set; }
        // public JObject? WifeLastName { get; set; }
        public string? WifeFirstNameOr { get; set; }
        public string? WifeFirstNameAm { get; set; }
        public string? WifeMiddleNameOr { get; set; }
        public string? WifeMiddleNameAm { get; set; }
        public string? WifeLastNameOr { get; set; }
        public string? WifeLastNameAm { get; set; }

        // public LookupDTO? WifeNationality { get; set; }
        public string? WifeNationalityOr { get; set; }
        public string? WifeNationalityAm { get; set; }

        // public JObject? HusbandFirstName { get; set; }
        // public JObject? HusbandMiddleName { get; set; }
        // public JObject? HusbandLastName { get; set; }

        public string? HusbandFirstNameOr { get; set; }
        public string? HusbandFirstNameAm { get; set; }
        public string? HusbandMiddleNameOr { get; set; }
        public string? HusbandMiddleNameAm { get; set; }
        public string? HusbandLastNameOr { get; set; }
        public string? HusbandLastNameAm { get; set; }

        // public LookupDTO? HusbandNationality { get; set; }
        public string? HusbandNationalityOr { get; set; }
        public string? HusbandNationalityAm { get; set; }

        // public DateTime? DivorceDate { get; set; }
        public string? DivorceMonth { get; set; }
        public string? DivorceDay { get; set; }
        public string? DivorceYear { get; set; }

        // public AddressDTO? DivorcePlace { get; set; }
        public string? DivorceAddressOr { get; set; }
        public string? DivorceAddressAm { get; set; }

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