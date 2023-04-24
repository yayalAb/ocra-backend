using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class GroupRepository : BaseRepository<UserGroup>, IGroupRepository
    {
        public GroupRepository(CRVSDbContext dbContext) : base(dbContext)
        {
        }

        async Task<UserGroup> IGroupRepository.GetByIdAsync(Guid id)
        {
            return await base.GetAsync(id);
        }
    }
}