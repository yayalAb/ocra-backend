using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class Person
    {
        public string? FirstNameAm { get; set; }
        public string? FirstNameOr { get; set; }
        public string? MiddleNameAm { get; set; }
        public string? MiddleNameOr { get; set; }
        public string? LastNameAm { get; set; }
        public string? LastNameOr { get; set; }

        public string? BirthMonthOr { get; set; }
        public string? BirthMonthAm { get; set; }
        public string? BirthDay { get; set; }
        public string? BirthYear { get; set; }

        public virtual string? GenderAm { get; set; }
        public virtual string? GenderOr { get; set; }

        public string? BirthAddressOr { get; set; }
        public string? BirthAddressAm { get; set; }

        public string? ResidentAddressOr { get; set; }
        public string? ResidentAddressAm { get; set; }

        public string? NationalId { get; set; }

        public string? NationalityOr { get; set; }
        public string? NationalityAm { get; set; }

        public string? MarriageStatusOr { get; set; }
        public string? MarriageStatusAm { get; set; }

        public string? ReligionOr { get; set; }
        public string? ReligionAm { get; set; }

        public string? NationOr { get; set; }
        public string? NationAm { get; set; }

        public string? EducationalStatusOr { get; set; }
        public string? EducationalStatusAm { get; set; }

        public string? TypeOfWorkOr { get; set; }
        public string? TypeOfWorkAm { get; set; }

        public string? BirthCountryOr { get; set; }
        public string? BirthCountryAm { get; set; }
        public string? BirthRegionOr { get; set; }
        public string? BirthRegionAm { get; set; }
        public string? BirthZoneOr { get; set; }
        public string? BirthZoneAm { get; set; }
        public string? BirthWoredaOr { get; set; }
        public string? BirthWoredaAm { get; set; }
         public string? BirthSubcityOr { get; set; }
        public string? BirthSubcityAm { get; set; }
        public string? BirthCityKetemaOr { get; set; }
        public string? BirthCityKetemaAm { get; set; }
        public string? BirthKebeleOr { get; set; }
        public string? BirthKebeleAm { get; set; }

        public string? ResidentCountryOr { get; set; }
        public string? ResidentCountryAm { get; set; }
        public string? ResidentRegionOr { get; set; }
        public string? ResidentRegionAm { get; set; }
        public string? ResidentZoneOr { get; set; }
        public string? ResidentZoneAm { get; set; }
        public string? ResidentSubcityOr { get; set; }
        public string? ResidentSubcityAm { get; set; }
        public string? ResidentWoredaOr { get; set; }
        public string? ResidentWoredaAm { get; set; }
        public string? ResidentCityKetemaOr { get; set; }
        public string? ResidentCityKetemaAm { get; set; }
        public string? ResidentKebeleOr { get; set; }
        public string? ResidentKebeleAm { get; set; }

    }
}