using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.DivorceArchive
{
    public class DivorceInfo : EventInfoArchive
    {
        public string? WifeBirthCertificateId { get; set; }
        public string? HusbandBirthCertificateId { get; set; }

        public string? MarriageMonthOr { get; set; }
        public string? MarriageMonthAm { get; set; }
        public string? MarriageDay { get; set; }
        public string? MarriageYear { get; set; }

        public string? MarriageAddressOr { get; set; }
        public string? MarriageAddressAm { get; set; }

        public string? DivorceReasonOr { get; set; }
        public string? DivorceReasonAm { get; set; }

        // public string? DivorceMonthOr { get; set; }
        // public string? DivorceMonthAm { get; set; }
        // public string? DivorceDay { get; set; }
        // public string? DivorceYear { get; set; }

        public CourtArchive Court { get; set; }

        public int? NumberOfChildren { get; set; }
    }
}