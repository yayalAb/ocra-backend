using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateRepository : BaseRepository<Certificate>, ICertificateRepository
    {
        private readonly CRVSDbContext _dbContext;
        public CertificateRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<IEnumerable<Certificate>> GetByEventAsync(Guid id)
        {
            return await _dbContext.Certificates.Where(c => c.EventId == id).ToListAsync();
        }

        public async Task<JObject> GetContent(Guid eventId)
        {
            return new JObject();

        }
    }
}