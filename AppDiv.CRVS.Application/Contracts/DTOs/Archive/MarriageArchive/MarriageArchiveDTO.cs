using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.MarriageArchive;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class MarriageArchiveDTO : BaseArchiveDTO
    {
        public Person? Bride { get; set; }
        public Person? Groom { get; set; }
        public MarriageInfo? EventInfo { get; set; }
        public Officer? CivilRegistrarOfficer { get; set; }

    }
}