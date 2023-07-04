using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Persistence.Couch;
public interface IAddressLookupCouchRepository
{
    public Task<bool> InserAsync(Address address);
    public Task<bool> UpdateAsync(Address Address);
    public Task<bool> RemoveAsync(Address Address);
    public Task<bool> BulkInsertAsync(IQueryable<Address> Address);
    public Task<bool> IsEmpty();



}