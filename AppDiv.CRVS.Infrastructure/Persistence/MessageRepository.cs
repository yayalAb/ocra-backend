
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        private readonly CRVSDbContext _dbContext;

        public MessageRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}