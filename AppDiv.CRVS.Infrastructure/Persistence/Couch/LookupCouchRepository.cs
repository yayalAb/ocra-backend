using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Infrastructure.CouchModels;

namespace AppDiv.CRVS.Infrastructure.Persistence.Couch;
public class LookupCouchRepository : ILookupCouchRepository
{
    private readonly CRVSCouchDbContext _couchContext;

    public LookupCouchRepository(CRVSCouchDbContext couchContext)
    {
        _couchContext = couchContext;
    }
    public async Task<bool> InsertLookupAsync(Lookup lookup)
    {
        var newLookup = new LookupCouch
        {
            Id = lookup.Id,
            Key = lookup.Key,
            ValueAm = lookup.Value?.Value<string>("am"),
            ValueOr = lookup.Value?.Value<string>("or")
        };
        try
        {

            await _couchContext.Lookups.AddAsync(newLookup);

        }
        catch (System.Exception)
        {

            throw;
        }
        return true;
    }
    public async Task<bool> BulkInsertAsync(List<Lookup> lookup)
    {
        var res = await _couchContext.Lookups.AddOrUpdateRangeAsync(lookup.Select(
         l => new LookupCouch
         {
             Id = l.Id,
             Key = l.Key,
             ValueAm = l.Value.Value<string>("am"),
             ValueOr = l.Value.Value<string>("or")
         }
        ).ToList());
        return true;
    }
    public async Task<bool> UpdateLookupAsync(Lookup lookup)
    {
        var existing = _couchContext.Lookups.Where(l => l.Id == lookup.Id).FirstOrDefault();
        if (existing != null)
        {

            existing.Id = lookup.Id;
            existing.Key = lookup.Key;
            existing.ValueAm = lookup.Value?.Value<string>("am");
            existing.ValueOr = lookup.Value?.Value<string>("or");
            var res = await _couchContext.Lookups.AddOrUpdateAsync(existing);
        }


        return true;
    }
    public async Task<bool> RemoveLookupAsync(Lookup lookup)
    {
        var existing = _couchContext.Lookups.Where(l => l.Id == lookup.Id).FirstOrDefault();
        if (existing != null)
        {

            await _couchContext.Lookups.RemoveAsync(existing);
        }
        // await _couchContext.Lookups.DeleteIndexAsync(deletedLookup, "")
        return true;
    }
    public async Task<bool> IsEmpty()
    {
        // return !(await _couchContext.);
        var count = _couchContext.Client.GetDatabase<LookupCouch>().ToList().Count;
        return count == 0;
    }
}