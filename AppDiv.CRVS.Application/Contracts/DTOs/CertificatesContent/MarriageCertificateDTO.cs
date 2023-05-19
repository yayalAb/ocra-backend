using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent
{
    public class MarriageCertificateDTO
    {
        public string? CertifcateId { get; set; }
        public string? RegBookNo { get; set; }
        public string? BirthCertifcateId { get; set; }

        public string? BrideFirstNameOr { get; set; }
        public string? BrideFirstNameAm { get; set; }
        public string? BrideMiddleNameOr { get; set; }
        public string? BrideMiddleNameAm { get; set; }
        public string? BrideLastNameOr { get; set; }
        public string? BrideLastNameAm { get; set; }

        public string? BrideNationalityOr { get; set; }
        public string? BrideNationalityAm { get; set; }

        public string? GroomFirstNameOr { get; set; }
        public string? GroomFirstNameAm { get; set; }
        public string? GroomMiddleNameOr { get; set; }
        public string? GroomMiddleNameAm { get; set; }
        public string? GroomLastNameOr { get; set; }
        public string? GroomLastNameAm { get; set; }

        public string? GroomNationalityOr { get; set; }
        public string? GroomNationalityAm { get; set; }

        public string? MarriageMonth { get; set; }
        public string? MarriageDay { get; set; }
        public string? MarriageYear { get; set; }

        public string? MarriageAddressOr { get; set; }
        public string? MarriageAddressAm { get; set; }

        public string? EventRegisteredMonth { get; set; }
        public string? EventRegisteredDay { get; set; }
        public string? EventRegisteredYear { get; set; }

        public string? GeneratedMonth { get; set; }
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