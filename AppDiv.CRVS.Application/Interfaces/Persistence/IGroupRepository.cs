using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface IGroupRepository : IBaseRepository<UserGroup>
    {
        public DatabaseFacade Database { get; }
        // Task<UserGroup> GetByIdAsync(Guid id);
        Task<List<UserGroup>> GetMultipleUserGroups(List<Guid> userIds);
    }
}