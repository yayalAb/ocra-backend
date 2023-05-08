using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MarriageApplicationRepository : BaseRepository<MarriageApplication>, IMarriageApplicationRepository
    {
        private readonly CRVSDbContext dbContext;

        public MarriageApplicationRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<MarriageApplication> GetAllQueryableAsync()
        {

            return dbContext.MarriageApplications.AsQueryable();
        }
        public void EFUpdate(MarriageApplication marriageApplication)
        {
            dbContext.MarriageApplications.Update(marriageApplication);
        }
        public bool exists(Guid id)
        {
            return dbContext.MarriageApplications.Where(m => m.Id == id).Any();
        }
    }
}