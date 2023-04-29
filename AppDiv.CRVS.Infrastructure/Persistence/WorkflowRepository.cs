

using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class WorkflowRepository : BaseRepository<Workflow>, IWorkflowRepository
    {
        private readonly CRVSDbContext dbContext;

        public WorkflowRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task<Workflow> IWorkflowRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
        public virtual async Task UpdateAsync(Workflow workflow)
        {

        }

    }
}