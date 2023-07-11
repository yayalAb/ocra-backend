using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Persistence.Couch;
public interface IAddressLookupCouchRepository
{
    public Task<bool> InserAsync(AddressCouchDTO address);
    public Task<bool> UpdateAsync(AddressCouchDTO Address);
    public Task<bool> RemoveAsync(AddressCouchDTO Address);
    public Task<bool> BulkInsertAsync(IQueryable<Address> Address);
    public Task<bool> IsEmpty();



}