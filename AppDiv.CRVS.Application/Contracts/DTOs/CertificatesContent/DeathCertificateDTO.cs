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
        
        public string? FirstNameAm { get; set; }
        public string? FirstNameOr { get; set; }

        public string? MiddleNameAm { get; set; }
        public string? MiddleNameOr { get; set; }

        public string? LastNameAm { get; set; }
        public string? LastNameOr { get; set; }
        public LookupDTO? Title { get; set; }

        public string? GenderAm { get; set; }
        public string? GenderOr { get; set; }

        public string? BirthMonth { get; set; }
        public string? BirthDay { get; set; }
        public string? BirthYear { get; set; }

        public string? DeathPlaceOr { get; set; }
        public string? DeathPlaceAm { get; set; }

        public string? DeathMonth { get; set; }
        public string? DeathDay { get; set; }
        public string? DeathYear { get; set; }

        public string? NationalityOr { get; set; }
        public string? NationalityAm { get; set; }

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