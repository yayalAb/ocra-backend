

using AppDiv.CRVS.Application.Interfaces;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class PersonDuplicateRepository :BaseRepository<PersonDuplicate>, IPersonDuplicateRepository
    {
        private readonly CRVSDbContext _dbContext;
        public PersonDuplicateRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}