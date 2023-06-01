using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class BirthArchiveDTO : BirthCertificateDTO
    {
        public float? ChildWeightAtBirth { get; set; }
        // public float? ChildWeightAtBirthAm { get; set; }

        public string? DeliveryTypeOr { get; set; }
        public string? DeliveryTypeAm { get; set; }

        public string? SkilledProfessionalOr { get; set; }
        public string? SkilledProfessionalAm { get; set; }

        public string? TypeOfBirthOr { get; set; }
        public string? TypeOfBirthAm { get; set; }

        public string? NotificationSerialNumber { get; set; }
        // public string? NotificationSerialNumberAm { get; set; }

        // Mother

        public string? MotherNationalId { get; set; }
        // public string? MotherNationalIdAm { get; set; }

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

    }
}