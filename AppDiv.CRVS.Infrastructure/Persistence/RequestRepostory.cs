using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class RequestRepostory : BaseRepository<Request>, IRequestRepostory
    {
        private readonly CRVSDbContext _DbContext;
        public RequestRepostory(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

    }
}