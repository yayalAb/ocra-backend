using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive
{
    public class DeathArchiveDTO : BaseArchiveDTO
    {
        public DeceasedPerson? Deceased { get; set; }
        public DeathInfo? EventInfo { get; set; }
        public DeathNotificationArchive? Notification { get; set; }
        public RegistrarArchive? Registrar { get; set; }
        public Officer? CivilRegistrarOfficer { get; set; }
    }
}