using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Archive.DeathArchive
{
    public class DeceasedPerson : Person
    {
        public int? Age { get; set; }
    }
}