using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FormatedAddressDto
    {
        public string? CountryOr { get; set; }
        public string? CountryAm { get; set; }
        public string? RegionOr { get; set; }
        public string? RegionAm { get; set; }
        public string? ZoneOr { get; set; }
        public string? ZoneAm { get; set; }
        public string? WoredaOr { get; set; }
        public string? WoredaAm { get; set; }
         public string? SubcityOr { get; set; }
        public string? SubcityAm { get; set; }
        public string? CityKetemaOr { get; set; }
        public string? CityKetemaAm { get; set; }
        public string? KebeleOr { get; set; }
        public string? KebeleAm { get; set; }

    }
}