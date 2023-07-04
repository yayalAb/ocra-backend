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
            ParentAddressId = address.ParentAddressId?.ToString() ?? "",
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
    public async Task<bool> BulkInsertAsync(IQueryable<Address> addresses)
    {
        var selected = addresses.GroupBy(a => a.ParentAddressId).Select(g  =>  new AddressCouch{
                            Id = g.Key,
                            addresses = g.Select(ca => new SingleAddressCouch{
                                Id = ca.Id ,
                                NameAm = ca.AddressName == null?null: ca.AddressName.Value<string>("am"),
                                NameOr =  ca.AddressName == null?null:ca.AddressName.Value<string>("or"),
                                AdminLevel = ca.AdminLevel,
                                AdminTypeAm =  ca.AdminTypeLookup == null?null:ca.AdminTypeLookup.Value.Value<string>("am"),
                                AdminTypeOr =  ca.AdminTypeLookup == null?null:ca.AdminTypeLookup.Value.Value<string>("or")
                            }).ToList()});

         var res = await _couchContext.AddressCouches.AddOrUpdateRangeAsync(selected.ToList());
        return true;
    }


    public async Task<bool> UpdateAsync(Address address)
    {
        var existing = _couchContext.Addresses.Where(a => a.Id == address.Id).FirstOrDefault();
        if (existing != null)
        {

            existing.Id = address.Id;
            existing.ParentAddressId = address.ParentAddressId?.ToString() ?? "null";
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
        var count = _couchContext.Client.GetDatabase<AddressCouch>().ToList().Count;
        return count == 0;
    }
}