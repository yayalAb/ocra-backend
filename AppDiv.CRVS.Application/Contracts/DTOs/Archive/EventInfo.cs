using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class EventInfoArchive
    {
        public string? EventMonthOr { get; set; }
        public string? EventMonthAm { get; set; }
        public string? EventDay { get; set; }
        public string? EventYear { get; set; }

        public string? RegistrationMonthOr { get; set; }
        public string? RegistrationMonthAm { get; set; }
        public string? RegistrationDay { get; set; }
        public string? RegistrationYear { get; set; }

        public string? EventCountryOr { get; set; }
        public string? EventCountryAm { get; set; }
        public string? EventRegionOr { get; set; }
        public string? EventRegionAm { get; set; }
        public string? EventZoneOr { get; set; }
        public string? EventZoneAm { get; set; }
        public string? EventWoredaOr { get; set; }
        public string? EventWoredaAm { get; set; }
        public string? EventCityKetemaOr { get; set; }
        public string? EventCityKetemaAm { get; set; }
        public string? EventKebeleOr { get; set; }
        public string? EventKebeleAm { get; set; }
    }
}