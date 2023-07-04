using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class AddressCouch : CouchDocument
{
    public Guid? Id { get; set; }
    public List<SingleAddressCouch> addresses {get;set; }
}