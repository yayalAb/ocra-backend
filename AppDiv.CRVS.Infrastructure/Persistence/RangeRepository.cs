using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class RangeRepository : BaseRepository<SystemRange>, IRangeRepository
    {
        private readonly CRVSDbContext _dbContext;

        public RangeRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<SystemRange> GetRanges()
        {
            return _dbContext.SystemRanges;
                // .Include(p => p.Address.ParentAddress.ParentAddress)
                // .Include(p => p.ParentPlan)
                // .ThenInclude(p => p.Address.ParentAddress.ParentAddress);
                
        }
    }

}