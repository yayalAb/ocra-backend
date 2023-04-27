

using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class StepRepository : BaseRepository<Step>, IStepRepository
    {
        public StepRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }
    }
}