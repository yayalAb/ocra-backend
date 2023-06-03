using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.AdoptionArchive;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class AdoptionArchiveDTO
    {
        public AdoptedChild? Child { get; set; }
        public Person? Father { get; set; }
        public Person? Mother { get; set; }
        public CourtArchive? Court { get; set; }
        public AdoptionInfo? EventInfo { get; set; }
        public Officer? CivilRegistrarOfficer { get; set; }

    }
}