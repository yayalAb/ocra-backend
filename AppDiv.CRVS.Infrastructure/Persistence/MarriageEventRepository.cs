using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MarriageEventRepository : BaseRepository<MarriageEvent>, IMarriageEventRepository
    {
        private readonly CRVSDbContext dbContext;

        public MarriageEventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<MarriageEvent> GetAllQueryableAsync()
        {

            return dbContext.MarriageEvents.AsQueryable();
        }
        public void EFUpdate(MarriageEvent marriageEvent)
        {
            dbContext.MarriageEvents.Update(marriageEvent);
        }
        public bool exists(Guid id){
            return dbContext.MarriageEvents.Where(m =>m.Id == id).Any();
        }
    }
}