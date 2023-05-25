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
        public string? WifeBirthCertifcateId { get; set; }

        public string? WifeFirstNameOr { get; set; }
        public string? WifeFirstNameAm { get; set; }
        public string? WifeMiddleNameOr { get; set; }
        public string? WifeMiddleNameAm { get; set; }
        public string? WifeLastNameOr { get; set; }
        public string? WifeLastNameAm { get; set; }

        public string? WifeNationalityOr { get; set; }
        public string? WifeNationalityAm { get; set; }

        public string? HusbandBirthCertifcateId { get; set; }
        public string? HusbandFirstNameOr { get; set; }
        public string? HusbandFirstNameAm { get; set; }
        public string? HusbandMiddleNameOr { get; set; }
        public string? HusbandMiddleNameAm { get; set; }
        public string? HusbandLastNameOr { get; set; }
        public string? HusbandLastNameAm { get; set; }

        public string? HusbandNationalityOr { get; set; }
        public string? HusbandNationalityAm { get; set; }

        public string? DivorceMonthOr { get; set; }
        public string? DivorceMonthAm { get; set; }
        public string? DivorceDay { get; set; }
        public string? DivorceYear { get; set; }

        public string? DivorceAddressOr { get; set; }
        public string? DivorceAddressAm { get; set; }

        public string? EventRegisteredMonthOr { get; set; }
        public string? EventRegisteredMonthAm { get; set; }
        public string? EventRegisteredDay { get; set; }
        public string? EventRegisteredYear { get; set; }

        public string? GeneratedMonthOr { get; set; }
        public string? GeneratedMonthAm { get; set; }
        public string? GeneratedDay { get; set; }
        public string? GeneratedYear { get; set; }

        public string? CivileRegOfficerFullNameOr { get; set; }
        public string? CivileRegOfficerFullNameAm { get; set; }

        //splited Address
        public string? CountryOr { get; set; }
        public string? CountryAm { get; set; }
        public string? RegionOr { get; set; }
        public string? RegionAm { get; set; }
        public string? ZoneOr { get; set; }
        public string? ZoneAm { get; set; }
        public string? WoredaOr { get; set; }
        public string? WoredaAm { get; set; }
        public string? CityOr { get; set; }
        public string? CityAm { get; set; }
        public string? KebeleOr { get; set; }
        public string? KebeleAm { get; set; }
    }
}