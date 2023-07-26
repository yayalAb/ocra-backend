using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Persistence.Couch;
public interface ILookupCouchRepository
{
    public Task<(Guid Id, string _Id)> InsertLookupAsync(LookupCouchDTO lookup);
    public Task<bool> UpdateLookupAsync(LookupCouchDTO lookup);
    public Task<bool> RemoveLookupAsync(LookupCouchDTO lookup);
    public Task<bool> BulkInsertAsync(List<Lookup> lookup);
    public Task<bool> IsEmpty();



}