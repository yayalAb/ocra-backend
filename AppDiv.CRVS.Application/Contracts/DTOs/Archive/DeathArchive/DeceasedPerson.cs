using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive
{
    public class DeceasedPerson : Person
    {
        public string? Age { get; set; }
        public string? TitileAm { get; set; }
        public string? TitileOr { get; set; }
    }
}