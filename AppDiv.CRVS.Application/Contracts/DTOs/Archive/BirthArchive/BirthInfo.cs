using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.BirthArchive
{
    public class BirthInfo : EventInfoArchive
    {
        public float? WeightAtBirth { get; set; }

        public string? DeliveryTypeOr { get; set; }
        public string? DeliveryTypeAm { get; set; }

        public string? SkilledProfessionalOr { get; set; }
        public string? SkilledProfessionalAm { get; set; }

        public string? TypeOfBirthOr { get; set; }
        public string? TypeOfBirthAm { get; set; }

        public string? NotificationSerialNumber { get; set; }
    }
}