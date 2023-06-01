using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class MarriageArchiveDTO : MarriageCertificateDTO
    {

        // Bride
        public string? BrideNationalIdOr { get; set; }
        public string? BrideNationalIdAm { get; set; }

        public string? BrideBirthAddressOr { get; set; }
        public string? BrideBirthAddressAm { get; set; }

        public string? BrideResidentAddressOr { get; set; }
        public string? BrideResidentAddressAm { get; set; }

        public string? BrideMarriageStatusOr { get; set; }
        public string? BrideMarriageStatusAm { get; set; }

        public string? BrideReligionOr { get; set; }
        public string? BrideReligionAm { get; set; }

        public string? BrideNationOr { get; set; }
        public string? BrideNationAm { get; set; }

        public string? BrideEducationalStatusOr { get; set; }
        public string? BrideEducationalStatusAm { get; set; }

        public string? BrideTypeOfWorkOr { get; set; }
        public string? BrideTypeOfWorkAm { get; set; }

        // Groom

        public string? GroomNationalIdOr { get; set; }
        public string? GroomNationalIdAm { get; set; }

        public string? GroomBirthAddressOr { get; set; }
        public string? GroomBirthAddressAm { get; set; }

        public string? GroomResidentAddressOr { get; set; }
        public string? GroomResidentAddressAm { get; set; }

        public string? GroomMarriageStatusOr { get; set; }
        public string? GroomMarriageStatusAm { get; set; }

        public string? GroomReligionOr { get; set; }
        public string? GroomReligionAm { get; set; }

        public string? GroomNationOr { get; set; }
        public string? GroomNationAm { get; set; }

        public string? GroomEducationalStatusOr { get; set; }
        public string? GroomEducationalStatusAm { get; set; }

        public string? GroomTypeOfWorkOr { get; set; }
        public string? GroomTypeOfWorkAm { get; set; }

        // Marriage

        public string? MarriageTypeOr { get; set; }
        public string? MarriageTypeAm { get; set; }

        public ICollection<WitnessArchiveDTO>? BrideWitnesses { get; set; }
        public ICollection<WitnessArchiveDTO>? GroomWitnesses { get; set; }

    }
}