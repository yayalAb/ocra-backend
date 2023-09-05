using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Archive
{
    public interface IReturnDeathArchive
    {
        public DeathArchiveDTO GetDeathArchive(Event death, string? BirthCertNo, bool IsCorrection=false);
        public DeathArchiveDTO GetDeathPreviewArchive(DeathEvent death, string? BirthCertNo, bool IsCorrection=false);
    }
}