using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.BirthArchive;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class BirthArchiveDTO
    {
        public Child? Child { get; set; }
        public Person? Mother { get; set; }
        public Person? Father { get; set; }

        public BirthInfo? EventInfo { get; set; }
        public RegistrarArchive? Registrar { get; set; }
        public Officer? CivilRegistrarOfficer { get; set; }
    }
}