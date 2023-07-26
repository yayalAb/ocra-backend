
using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class EventDuplicateRepository: BaseRepository<EventDuplicate>, IEventDuplicateRepository
    {
        private readonly CRVSDbContext _dbContext;
        public EventDuplicateRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

    }
}