using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Persistence.Couch;
public interface ILookupCouchRepository
{
    public Task<bool> InsertLookupAsync(Lookup lookup);
    public Task<bool> UpdateLookupAsync(Lookup lookup);
    public Task<bool> RemoveLookupAsync(Lookup lookup);
    public Task<bool> BulkInsertAsync(List<Lookup> lookup);
    public Task<bool> IsEmpty();



}