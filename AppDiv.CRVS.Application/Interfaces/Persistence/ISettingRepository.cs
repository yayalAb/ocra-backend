using AppDiv.CRVS.Application.Interfaces.Persistence.Base;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence
{
    public interface ISettingRepository : IBaseRepository<Setting>
    {
        Task<IEnumerable<Setting>> GetAllAsync();
        Task<Setting> GetByIdAsync(Guid id);
        Task<Setting> GetSettingByKey(string key);
         Task InitializeSettingCouch();


    }
}

