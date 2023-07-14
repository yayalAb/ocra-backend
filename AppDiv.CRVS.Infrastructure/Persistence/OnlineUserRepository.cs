using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class OnlineUserRepository: BaseRepository<OnlineUser>, IOnlineUserRepository
    {
        private readonly CRVSDbContext _dbContext;

        public OnlineUserRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }

}