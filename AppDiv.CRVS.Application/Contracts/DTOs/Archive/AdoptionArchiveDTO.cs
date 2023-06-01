using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.CertificatesContent;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class AdoptionArchiveDTO : AdoptionCertificateDTO
    {
        public string? ChildResidentAddressOr { get; set; }
        public string? ChildResidentAddressAm { get; set; }

        public string? ChildEducationalStatusOr { get; set; }
        public string? ChildEducationalStatusAm { get; set; }

        public string ReasonOr { get; set; }
        public string ReasonAm { get; set; }

        // Mother
        public string? MotherNationalIdOr { get; set; }
        public string? MotherNationalIdAm { get; set; }

        public string? MotherBirthMonthOr { get; set; }
        public string? MotherBirthMonthAm { get; set; }
        public string? MotherBirthDay { get; set; }
        public string? MotherBirthYear { get; set; }

        public string? MotherBirthAddressOr { get; set; }
        public string? MotherBirthAddressAm { get; set; }

        public string? MotherResidentAddressOr { get; set; }
        public string? MotherResidentAddressAm { get; set; }

        public string? MotherMarriageStatusOr { get; set; }
        public string? MotherMarriageStatusAm { get; set; }

        public string? MotherReligionOr { get; set; }
        public string? MotherReligionAm { get; set; }

        public string? MotherNationOr { get; set; }
        public string? MotherNationAm { get; set; }

        public string? MotherEducationalStatusOr { get; set; }
        public string? MotherEducationalStatusAm { get; set; }

        public string? MotherTypeOfWorkOr { get; set; }
        public string? MotherTypeOfWorkAm { get; set; }


        //Father
        public string? FatherNationalIdOr { get; set; }
        public string? FatherNationalIdAm { get; set; }

        public string? FatherBirthMonthOr { get; set; }
        public string? FatherBirthMonthAm { get; set; }
        public string? FatherBirthDay { get; set; }
        public string? FatherBirthYear { get; set; }

        public string? FatherBirthAddressOr { get; set; }
        public string? FatherBirthAddressAm { get; set; }

        public string? FatherResidentAddressOr { get; set; }
        public string? FatherResidentAddressAm { get; set; }

        public string? FatherMarriageStatusOr { get; set; }
        public string? FatherMarriageStatusAm { get; set; }

        public string? FatherReligionOr { get; set; }
        public string? FatherReligionAm { get; set; }

        public string? FatherNationOr { get; set; }
        public string? FatherNationAm { get; set; }

        public string? FatherEducationalStatusOr { get; set; }
        public string? FatherEducationalStatusAm { get; set; }

        public string? FatherTypeOfWorkOr { get; set; }
        public string? FatherTypeOfWorkAm { get; set; }

        // Court
        public string? CourtNameOr { get; set; }
        public string? CourtNameAm { get; set; }

        public string? CourtAddressOr { get; set; }
        public string? CourtAddressAm { get; set; }

        public string? CourtConfirmedMonthOr { get; set; }
        public string? CourtConfirmedMonthAm { get; set; }
        public string? CourtConfirmedDay { get; set; }
        public string? CourtConfirmedYear { get; set; }

        public string CourtCaseNumber { get; set; }



    }
}