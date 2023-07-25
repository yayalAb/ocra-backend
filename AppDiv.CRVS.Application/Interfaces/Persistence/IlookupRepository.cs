using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ILookupRepository : IBaseRepository<Lookup>
    {
        Task<IEnumerable<Lookup>> GetAllAsync();
        Task<Lookup> GetByIdAsync(Guid id);
        Task<Lookup> GetLookupByKey(string key);
        Task<Lookup?> GetLookupById(Guid id);
        Task InitializeLookupCouch();
        Task Import(ICollection<Lookup> lookups, CancellationToken cancellationToken);
         Task<(Guid Id, string _Id)> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

