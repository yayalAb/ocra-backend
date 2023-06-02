using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.MarriageArchive
{
    public class MarriageInfo : EventInfo
    {
        public string? MarriageTypeOr { get; set; }
        public string? MarriageTypeAm { get; set; }

        public ICollection<WitnessArchive?>? BrideWitnesses { get; set; }
        public ICollection<WitnessArchive?>? GroomWitnesses { get; set; }
    }
}