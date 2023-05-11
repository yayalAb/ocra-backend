using AppDiv.CRVS.Domain;
using AppDiv.CRVS.Domain.Repositories;


namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class UserRepository : BaseRepository<ApplicationUser>, IUserRepository
    {
        private readonly CRVSDbContext dbContext;

        public UserRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
          public IQueryable<ApplicationUser> GetAllQueryableAsync()
        {

            return dbContext.Users.AsQueryable();
        }


    }
}