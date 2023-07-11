using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Application.Interfaces.Persistence.Couch;

namespace AppDiv.CRVS.Infrastructure.Persistence.Couch;
public class SettingCouchRepository : ISettingCouchRepository
{
    private readonly CRVSCouchDbContext _couchContext;

    public SettingCouchRepository(CRVSCouchDbContext couchContext)
    {
        _couchContext = couchContext;
    }
    public async Task<bool> InsertSettingAsync(SettingDTO Setting)
    {
        var newSetting = new SettingCouch
        {
            Id = Setting.Id,
            Key = Setting.Key,
            Value = Setting.Value,
            status = true,
        };
            await _couchContext.Settings.AddAsync(newSetting);

 
        return true;
    }
    public async Task<bool> BulkInsertAsync(IQueryable<Setting> Settings)
    {
        var res = await _couchContext.Settings.AddOrUpdateRangeAsync(Settings.Select(
         l => new SettingCouch
         {
             Id = l.Id,
             Key = l.Key,
             Value = l.Value,
             status = true
         }
        ).ToList());
        return true;
    }
    public async Task<bool> UpdateSettingAsync(SettingDTO Setting)
    {
        var existing = _couchContext.Settings.Where(l => l.Id == Setting.Id).FirstOrDefault();
        if (existing != null)
        {

            existing.Key = Setting.Key;
            existing.Value = Setting.Value;
            var res = await _couchContext.Settings.AddOrUpdateAsync(existing);
        }


        return true;
    }
    public async Task<bool> RemoveSettingAsync(SettingDTO Setting)
    {
        var existing = _couchContext.Settings.Where(l => l.Id == Setting.Id).FirstOrDefault();
        if (existing != null)
        {
            existing.status = false;
            await _couchContext.Settings.AddOrUpdateAsync(existing);
        }
        return true;
    }
    public async Task<bool> IsEmpty()
    {
        var count = _couchContext.Client.GetDatabase<SettingCouch>().ToList().Count;
        return count == 0;
    }
}