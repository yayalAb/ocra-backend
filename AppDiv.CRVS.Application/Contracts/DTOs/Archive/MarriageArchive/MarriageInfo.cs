using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.MarriageArchive
{
    public class MarriageInfo : EventInfoArchive
    {
        public string? BrideBirthCertificateId { get; set; }
        public string? GroomBirthCertificateId { get; set; }

        public string? MarriageTypeOr { get; set; }
        public string? MarriageTypeAm { get; set; }

        public ICollection<WitnessArchive?>? BrideWitnesses { get; set; } = new List<WitnessArchive?>();
        public ICollection<WitnessArchive?>? GroomWitnesses { get; set; } = new List<WitnessArchive?>();
    }
}