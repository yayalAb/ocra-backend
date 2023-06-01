using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class DivorceArchiveDTO : DivorceCertificateDTO
    {
        // Wife
        public string? WifeNationalIdOr { get; set; }
        public string? WifeNationalIdAm { get; set; }

        public string? WifeResidentAddressOr { get; set; }
        public string? WifeResidentAddressAm { get; set; }

        public string? WifeMarriageStatusOr { get; set; }
        public string? WifeMarriageStatusAm { get; set; }

        public string? WifeReligionOr { get; set; }
        public string? WifeReligionAm { get; set; }

        public string? WifeNationOr { get; set; }
        public string? WifeNationAm { get; set; }

        public string? WifeEducationalStatusOr { get; set; }
        public string? WifeEducationalStatusAm { get; set; }

        public string? WifeTypeOfWorkOr { get; set; }
        public string? WifeTypeOfWorkAm { get; set; }

        // Husband

        public string? HusbandNationalIdOr { get; set; }
        public string? HusbandNationalIdAm { get; set; }

        public string? HusbandResidentAddressOr { get; set; }
        public string? HusbandResidentAddressAm { get; set; }

        public string? HusbandMarriageStatusOr { get; set; }
        public string? HusbandMarriageStatusAm { get; set; }

        public string? HusbandReligionOr { get; set; }
        public string? HusbandReligionAm { get; set; }

        public string? HusbandNationOr { get; set; }
        public string? HusbandNationAm { get; set; }

        public string? HusbandEducationalStatusOr { get; set; }
        public string? HusbandEducationalStatusAm { get; set; }

        public string? HusbandTypeOfWorkOr { get; set; }
        public string? HusbandTypeOfWorkAm { get; set; }

        // Divorce

        public string? MarriageMonthOr { get; set; }
        public string? MarriageMonthAm { get; set; }
        public string? MarriageDay { get; set; }
        public string? MarriageYear { get; set; }

        public string? MarriageAddressOr { get; set; }
        public string? MarriageAddressAm { get; set; }

        public string? DivorceReasonOr { get; set; }
        public string? DivorceReasonAm { get; set; }

        // Court

        public string? CourtNameOr { get; set; }
        public string? CourtNameAm { get; set; }

        // public string? CourtAddressOr { get; set; }
        // public string? CourtAddressAm { get; set; }

        // public string? CourtConfirmedMonthOr { get; set; }
        // public string? CourtConfirmedMonthAm { get; set; }
        // public string? CourtConfirmedDay { get; set; }
        // public string? CourtConfirmedYear { get; set; }

        public string? CourtCaseNumber { get; set; }
        public string? NumberOfChildren { get; set; }




    }
}