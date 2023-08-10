
using AppDiv.CRVS.Application.CouchModels;
using AppDiv.CRVS.Application.Features.MarriageApplications.Command.Create;
using AppDiv.CRVS.Application.Interfaces.Persistence;
using AppDiv.CRVS.Application.Mapper;
using AppDiv.CRVS.Application.Persistence.Couch;
using AppDiv.CRVS.Domain.Entities;
using AppDiv.CRVS.Infrastructure.Context;
using CouchDB.Driver.Extensions;

namespace AppDiv.CRVS.Infrastructure.Persistence.Couch;
public class MarriageApplicationCouchRepository : IMarriageApplicationCouchRepository
{
    private readonly CRVSCouchDbContext _couchContext;
    private readonly IMarriageApplicationRepository _marriageApplicationRepo;

    public MarriageApplicationCouchRepository(CRVSCouchDbContext couchContext, IMarriageApplicationRepository marriageApplicationRepo)
    {
        _couchContext = couchContext;
        _marriageApplicationRepo = marriageApplicationRepo;
    }

    public async Task<(bool Success, string? Message, MarriageApplicationCouch? marriageApplication)> Exists(Guid applicationId, Guid addressId)
    {
        var dbName = (await _couchContext.Client.GetDatabasesNamesAsync())
                                        .Where(n => n.ToLower() == "marriageappplicationcouches" + addressId.ToString())
                                        .FirstOrDefault();
        if (dbName == null)
        {
            return (Success: false, Message: $"database not found in couch for the addressId -- {addressId}", marriageApplication: null);
        }
        var marriageApplicationCouches = _couchContext.Client.GetDatabase<MarriageApplicationCouch>(dbName);
        var marriageApplication = await marriageApplicationCouches.Where(m => m.Id == applicationId.ToString()).FirstOrDefaultAsync();
        if (marriageApplication == null)
        {
            return (Success: false, Message: $"marriage application with id {applicationId} is not found in couchdb", marriageApplication: null);
        }
        return (Success: true, Message: null, marriageApplication: marriageApplication);
    }

    public async Task<(bool Success, string? Message)> SyncMarraigeApplication(MarriageApplicationCouch marriageApplicationCouch, Guid addressId, CancellationToken cancellationToken)
    {
        var dbName = (await _couchContext.Client.GetDatabasesNamesAsync())
                                       .Where(n => n.ToLower() == "marriageappplicationcouches" + addressId.ToString())
                                       .FirstOrDefault();
        if (dbName == null)
        {
            return (Success: false, Message: $"database not found in couch for the addressId -- {addressId}");
        }
        var marriageApplicationCouches = _couchContext.Client.GetDatabase<MarriageApplicationCouch>(dbName);
        var marriageApplicationCommand
                     = new CreateMarriageApplicationCommand
                     {
                         Id = new Guid(marriageApplicationCouch.Id),
                         ApplicationDateEt = marriageApplicationCouch.ApplicationDateEt,
                         ApplicationAddressId = marriageApplicationCouch.ApplicationAddressId,
                         BrideInfo = marriageApplicationCouch.BrideInfo,
                         GroomInfo = marriageApplicationCouch.GroomInfo,
                         CivilRegOfficerId = marriageApplicationCouch.CivilRegOfficerId
                     };
        var marriageApplication = CustomMapper.Mapper.Map<MarriageApplication>(marriageApplicationCommand);
        await _marriageApplicationRepo.InsertAsync(
                            marriageApplication,
                            cancellationToken);
        var res = await _marriageApplicationRepo.SaveChangesAsync(cancellationToken);

        if (!res)
        {
            return (Success: false, Message: $"could not sync marraige application with id -- {marriageApplicationCouch.Id}");

        }

        marriageApplicationCouch.Synced = true;
        await marriageApplicationCouches.AddOrUpdateAsync(marriageApplicationCouch);
        return (Success: true, Message: null);



    }
}