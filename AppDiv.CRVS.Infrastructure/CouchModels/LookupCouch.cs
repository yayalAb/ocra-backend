using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class LookupCouch : CouchDocument{
    public Guid Id {get; set; }
    public string? Key {get; set; }
    public string? ValueAm { get; set; }
    public string? ValueOr {get; set; }
} 