
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Interfaces.Persistence;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class CertificateRangeRepository : BaseRepository<CertificateSerialRange>, ICertificateRangeRepository
    {
        private readonly CRVSDbContext _dbContext;
        public CertificateRangeRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}