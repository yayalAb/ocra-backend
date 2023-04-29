using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppDiv.CRVS.Infrastructure.Persistence
{
    public class GroupRepository : BaseRepository<UserGroup>, IGroupRepository
    {
        private readonly CRVSDbContext dbContext;

        public GroupRepository(CRVSDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

        // async Task<UserGroup> IGroupRepository.GetByIdAsync(Guid id)
        // {
        //     return await base.GetAsync(id);
        // }
        public async Task<List<UserGroup>> GetMultipleUserGroups(List<Guid> userIds)
        {
            return await dbContext.UserGroups.Where(ug => userIds.Contains(ug.Id)).ToListAsync();
        }
    }
}