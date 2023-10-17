using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class SharedReportRepository : BaseRepository<SharedReport>, ISharedReportRepository
    {
        private readonly CRVSDbContext dbContext;

        public SharedReportRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<SharedReport> ISharedReportRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }


    }
}