
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class ProfileChangeRequestRepository : BaseRepository<ProfileChangeRequest>, IProfileChangeRequestRepository
    {
        private readonly CRVSDbContext _DbContext;
          public DatabaseFacade Database => _DbContext.Database;

        public ProfileChangeRequestRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }
       


    }
}