using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs.Archive;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Archive
{
    public interface IReturnDivorceArchive
    {
        public DivorceArchiveDTO GetDivorceArchive(Event divorce, string? BirthCertNo, bool IsCorrection=false);
        public DivorceArchiveDTO GetDivorcePreviewArchive(DivorceEvent divorce, string? BirthCertNo,bool IsCorrection=false);
    }
}