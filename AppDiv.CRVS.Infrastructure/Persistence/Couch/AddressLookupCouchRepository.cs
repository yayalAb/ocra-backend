using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Context;
using AppDiv.CRVS.Infrastructure.CouchModels;

namespace AppDiv.CRVS.Infrastructure.Persistence.Couch;
public class AddressLookupCouchRepository : IAddressLookupCouchRepository
{
    private readonly CRVSCouchDbContext _couchContext;

    public AddressLookupCouchRepository(CRVSCouchDbContext couchContext)
    {
        _couchContext = couchContext;
    }
    public async Task<bool> InserAsync(Address address)
    {
        var newAddress = new AddressLookupCouch
        {
            Id = address.Id,
            ParentAddressId = address.ParentAddressId,
            NameAm = address.AddressName?.Value<string>("am"),
            NameOr = address.AddressName?.Value<string>("or")
        };
        try
        {

            await _couchContext.Addresses.AddAsync(newAddress);

        }
        catch (System.Exception)
        {

            throw;
        }
        return true;
    }
    public async Task<bool> BulkInsertAsync(List<Address> addresses)
    {
        var res = await _couchContext.Addresses.AddOrUpdateRangeAsync(addresses.Select(
         address => new AddressLookupCouch
         {
            Id = address.Id,
            ParentAddressId = address.ParentAddressId,
            NameAm = address.AddressName?.Value<string>("am"),
            NameOr = address.AddressName?.Value<string>("or")
        }
        ).ToList());
        return true;
    }
    public async Task<bool> UpdateAsync(Address address)
    {
        var existing = _couchContext.Addresses.Where(a => a.Id == address.Id).FirstOrDefault();
        if (existing != null)
        {

            existing.Id = address.Id;
            existing.ParentAddressId = address.ParentAddressId;
            existing.NameAm = address.AddressName?.Value<string>("am");
            existing.NameOr = address.AddressName?.Value<string>("or");
            var res = await _couchContext.Addresses.AddOrUpdateAsync(existing);
        }


        return true;
    }
    public async Task<bool> RemoveAsync(Address address)
    {
        var existing = _couchContext.Addresses.Where(l => l.Id == address.Id).FirstOrDefault();
        if (existing != null)
        {

            await _couchContext.Addresses.RemoveAsync(existing);
        }
        // await _couchContext.Lookups.DeleteIndexAsync(deletedLookup, "")
        return true;
    }
    public async Task<bool> IsEmpty()
    {
        // return !(await _couchContext.);
        var count = _couchContext.Client.GetDatabase<AddressLookupCouch>().ToList().Count;
        return count == 0;
    }
}