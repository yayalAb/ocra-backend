using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class CountryCouch : CouchDocument
{
    public Guid Id { get; set; }
    public string? NameAm { get; set; }
    public string? NameOr { get; set; }
}