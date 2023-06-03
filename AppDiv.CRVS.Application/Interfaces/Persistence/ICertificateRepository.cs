using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ICertificateRepository : IBaseRepository<Certificate>
    {
        Task<IEnumerable<Certificate>> GetByEventAsync(Guid id);
        Task<Certificate> GetByIdAsync(Guid id);
        // Task<Event?>? GetArchive(Guid id);
        Task<(BirthEvent? birth, DeathEvent? death, AdoptionEvent? adoption, MarriageEvent? marriage, DivorceEvent? divorce)> GetContent(Guid eventId);
    }
}