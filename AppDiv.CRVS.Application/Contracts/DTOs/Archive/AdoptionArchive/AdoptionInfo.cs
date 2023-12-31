using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.AdoptionArchive
{
    public class AdoptionInfo : EventInfoArchive
    {
        public string ReasonOr { get; set; }
        public string ReasonAm { get; set; }

        public string? BirthCertificateId { get; set; }
    }
}