using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ILookupRepository : IBaseRepository<LookupModel>
    {
        Task<IEnumerable<LookupModel>> GetAllAsync();
        Task<LookupModel> GetByIdAsync(Guid id);
        Task<LookupModel> GetLookupByKey(string key);
        Task<LookupModel> GetLookupListByKey(string[] key);
    }
}

