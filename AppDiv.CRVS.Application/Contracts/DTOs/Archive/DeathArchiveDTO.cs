using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class DeathArchiveDTO : DeathCertificateDTO
    {
        public string? ResidentAddressOr { get; set; }
        public string? ResidentAddressAm { get; set; }

        public string? NationOr { get; set; }
        public string? NationAm { get; set; }

        public string? ReligionOr { get; set; }
        public string? ReligionAm { get; set; }

        public string? EducationalStatusOr { get; set; }
        public string? EducationalStatusAm { get; set; }

        public string? TypeOfWorkOr { get; set; }
        public string? TypeOfWorkAm { get; set; }

        public string? MarriageStatusOr { get; set; }
        public string? MarriageStatusAm { get; set; }

        public string? PlaceOfFuneralOr { get; set; }
        public string? PlaceOfFuneralAm { get; set; }

        public string? RegistrarFirstNameAm { get; set; }
        public string? RegistrarFirstNameOr { get; set; }

        public string? RegistrarMiddleNameAm { get; set; }
        public string? RegistrarMiddleNameOr { get; set; }

        public string? RegistrarLastNameAm { get; set; }
        public string? RegistrarLastNameOr { get; set; }

        public string? RegistrarResidentAddressOr { get; set; }
        public string? RegistrarResidentAddressAm { get; set; }

        public string? RegistrarRelationShipOr { get; set; }
        public string? RegistrarRelationShipAm { get; set; }

        // public string? RegistrarNationalIdOr { get; set; }
        public string? RegistrarNationalId { get; set; }

        public string? DuringDeathOr { get; set; }
        public string? DuringDeathAm { get; set; }

        // public string? CauseOfDeathOr { get; set; }
        public string? CauseOfDeath { get; set; }

        public string? CauseOfDeathInfoTypeOr { get; set; }
        public string? CauseOfDeathInfoTypeAm { get; set; }

        public string? DeathNotificationSerialNumberOr { get; set; }
        public string? DeathNotificationSerialNumberAm { get; set; }

    }
}