using AppDiv.CRVS.Infrastructure.CouchModels;
using AppDiv.CRVS.Utility.Config;
using CouchDB.Driver;
using CouchDB.Driver.Options;
using Microsoft.Extensions.Options;

namespace AppDiv.CRVS.Infrastructure.Context;
public class CRVSCouchDbContext : CouchContext
{
    // private readonly CouchDbConfiguration _couchConfig;

    public CouchDatabase<LookupCouch> Lookups { get; set; }
    public CouchDatabase<AddressLookupCouch> Addresses { get; set; }
    public CouchDatabase<AddressCouch> AddressCouches { get; set; }
    public CouchDatabase<SettingCouch> Settings { get; set; }   
    public CouchDatabase<CountryCouch> Countries { get; set; }
    public CouchDatabase<PaymentRateCouch> PaymentRates { get; set; }



    // public CRVSCouchDbContext(IOptions<CouchDbConfiguration> couchConfig)
    // {
    //     _couchConfig = couchConfig.Value;
    // }
    protected override void OnConfiguring(CouchOptionsBuilder optionsBuilder)
    {
        optionsBuilder
          .UseEndpoint("https://couchdb.ocra.gov.et/")
          .EnsureDatabaseExists()
          .UseBasicAuthentication(username:"admin", password: "admin");
    }
}