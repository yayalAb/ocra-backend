using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Interfaces.Persistence.Couch
{
    public interface ISettingCouchRepository
    {
        public Task<bool> InsertSettingAsync(SettingDTO Setting);
        public Task<bool> UpdateSettingAsync(SettingDTO Setting);
        public Task<bool> RemoveSettingAsync(SettingDTO Setting);
        public Task<bool> BulkInsertAsync(IQueryable<Setting> Settings);
        public Task<bool> IsEmpty();

    }
}