using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive
{
    public class DeathInfo : EventInfoArchive
    {
        public string? BirthCertificateId { get; set; }

        public string? DuringDeathOr { get; set; }
        public string? DuringDeathAm { get; set; }

        public string? FacilityTypeOr { get; set; }
        public string? FacilityTypeAm { get; set; }

        public string? PlaceOfFuneralAm { get; set; }
        public string? PlaceOfFuneralOr { get; set; }

        public string? EventCountryOr { get; set; }
        public string? EventCountryAm { get; set; }
        public string? EventRegionOr { get; set; }
        public string? EventRegionAm { get; set; }
        public string? EventZoneOr { get; set; }
        public string? EventZoneAm { get; set; }
        public string? EventSubcityOr { get; set; }
        public string? EventSubcityAm { get; set; }
        public string? EventWoredaOr { get; set; }
        public string? EventWoredaAm { get; set; }
        public string? EventKebeleOr { get; set; }
        public string? EventKebeleAm { get; set; }
        public string? EventCityKetemaOr { get; set; }
        public string? EventCityKetemaAm { get; set; }

    }

    public class DeathNotificationArchive
    {
        public ICollection<string>? CauseOfDeath { get; set; }
        // public string? CauseOfDeathTwo { get; set; }
        // public string? CauseOfDeathThree { get; set; }

        public string? CauseOfDeathInfoTypeOr { get; set; }
        public string? CauseOfDeathInfoTypeAm { get; set; }

        public string? DeathNotificationSerialNumber { get; set; }
    }
}