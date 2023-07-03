using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class AddressLookupCouch : CouchDocument
{
    public Guid Id { get; set; }
    public Guid? ParentAddressId { get; set; }
    public string? NameAm { get; set; }
    public string? NameOr { get; set; }
}