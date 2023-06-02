using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive
{
    public class DeathInfo : EventInfo
    {
        public string? CauseOfDeath { get; set; }

        public string? CauseOfDeathInfoTypeOr { get; set; }
        public string? CauseOfDeathInfoTypeAm { get; set; }
        public string? DeathNotificationSerialNumber { get; set; }

        public string? DuringDeathOr { get; set; }
        public string? DuringDeathAm { get; set; }

        public string? PlaceOfFuneralOr { get; set; }
        public string? PlaceOfFuneralAm { get; set; }

    }
}