using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Archive
{
    public interface IReturnMarriageArchive
    {
        public MarriageArchiveDTO GetMarriageArchive(Event marriage, string? BirthCertNo, bool IsCorrection=false);
        public MarriageArchiveDTO GetMarriagePreviewArchive(MarriageEvent marriage, string? BirthCertNo, bool IsCorrection=false);
    }
}