using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Features.Search;
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
        public Task<List<SearchCertificateResponseDTO>> SearchCertificate(SearchCertificateQuery query);
        public Task InitializeCertificateIndex();
        Task<(BirthEvent? birth, DeathEvent? death, AdoptionEvent? adoption, MarriageEvent? marriage, DivorceEvent? divorce)> GetContent(Guid eventId);
    }
}