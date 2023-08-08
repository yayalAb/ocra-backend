
using AppDiv.CRVS.Application.CouchModels;

namespace AppDiv.CRVS.Application.Persistence.Couch;
public interface IMarriageApplicationCouchRepository
{
    public Task<(bool Success, string? Message, MarriageApplicationCouch? marriageApplication)> Exists(Guid applicationId, Guid addressId);

    public Task<(bool Success, string? Message)> SyncMarraigeApplication(MarriageApplicationCouch marriageApplicationCouch, Guid addressId, CancellationToken cancellationToken);


}