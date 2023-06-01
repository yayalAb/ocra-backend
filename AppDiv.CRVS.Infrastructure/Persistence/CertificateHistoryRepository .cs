using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateHistoryRepository : BaseRepository<CertificateHistory>, ICertificateHistoryRepository
    {
        private readonly CRVSDbContext _DbContext;
        public CertificateHistoryRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

        Task<IEnumerable<CertificateHistory>> ICertificateHistoryRepository.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        async Task<CertificateHistory> ICertificateHistoryRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
    }
}
