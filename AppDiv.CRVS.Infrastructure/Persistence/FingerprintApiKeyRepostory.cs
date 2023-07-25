using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class FingerprintApiKeyRepostory : BaseRepository<FingerprintApiKey>, IFingerprintApiKeyRepostory
    {
        private readonly CRVSDbContext dbContext;

        public FingerprintApiKeyRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}