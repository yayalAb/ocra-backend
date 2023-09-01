using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Application.Contracts.DTOs;
using CouchDB.Driver.Query.Extensions;

namespace AppDiv.CRVS.Infrastructure.Persistence.Couch
{
    public class LookupCouchRepository : ILookupCouchRepository
    {
        private readonly CRVSCouchDbContext _couchContext;

        public LookupCouchRepository(CRVSCouchDbContext couchContext)
        {
            _couchContext = couchContext;
        }
        public async Task<(Guid Id, string _Id)> InsertLookupAsync(LookupCouchDTO lookup)
        {
            var newLookup = new LookupCouch
            {
                Id2 = lookup.Id,
                Key = lookup.Key,
                ValueAm = lookup.Value?.Value<string>("am"),
                ValueOr = lookup.Value?.Value<string>("or"),
                ValueEn = lookup.Value?.Value<string>("en"),

                status = true,
            };
            try
            {

                await _couchContext.Lookups.AddAsync(newLookup);

            }
            catch (System.Exception)
            {

                throw;
            }
            return (Id: newLookup.Id2, _Id: newLookup.Id);
        }
        public async Task<bool> BulkInsertAsync(List<Lookup> lookups)
        {
            var res = await _couchContext.Lookups.AddOrUpdateRangeAsync(lookups.Select(
             l => new LookupCouch
             {
                 Id2 = l.Id,
                 Key = l.Key,
                 ValueAm = l.Value.Value<string>("am"),
                 ValueOr = l.Value.Value<string>("or"),
                 ValueEn = l.Value?.Value<string>("en"),
                 status = true
             }
            ).ToList());
            return true;
        }
        public async Task<bool> UpdateLookupAsync(LookupCouchDTO lookup)
        {
            var existing = _couchContext.Lookups.Where(l => l.Id2 == lookup.Id).FirstOrDefault();
            if (existing != null)
            {

                existing.Id2 = lookup.Id;
                existing.Key = lookup.Key;
                existing.ValueAm = lookup.Value?.Value<string>("am");
                existing.ValueOr = lookup.Value?.Value<string>("or");
                existing.ValueEn = lookup.Value?.Value<string>("en");

                var res = await _couchContext.Lookups.AddOrUpdateAsync(existing);
            }


            return true;
        }
        public async Task<bool> RemoveLookupAsync(LookupCouchDTO lookup)
        {
            var existing = _couchContext.Lookups.Where(l => l.Id2 == lookup.Id).FirstOrDefault();
            if (existing != null)
            {
                existing.status = false;
                await _couchContext.Lookups.AddOrUpdateAsync(existing);
                // await _couchContext.Lookups.RemoveAsync(existing);
            }
            // await _couchContext.Lookups.DeleteIndexAsync(deletedLookup, "")
            return true;
        }
        public async Task<bool> IsEmpty()
        {
            // return !(await _couchContext.);
            var dbInfo = await _couchContext.Client.GetDatabase<LookupCouch>().GetInfoAsync();
            var count = dbInfo.DocCount;
            return count == 0;
        }

    }
}