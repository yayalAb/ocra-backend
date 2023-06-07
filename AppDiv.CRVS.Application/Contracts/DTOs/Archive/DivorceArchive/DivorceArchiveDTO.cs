using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.DivorceArchive;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class DivorceArchiveDTO
    {
        public Person? Wife { get; set; }
        public Person? Husband { get; set; }
        public DivorceInfo? EventInfo { get; set; }
        public CourtArchive Court { get; set; }
        public Officer CivilRegistrarOfficer { get; set; }
    }
}