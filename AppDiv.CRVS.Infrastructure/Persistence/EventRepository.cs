using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        private readonly CRVSDbContext dbContext;

        public EventRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public IQueryable<Event> GetAllQueryableAsync()
        {

            return dbContext.Events.AsQueryable();
        }
       
    }
}