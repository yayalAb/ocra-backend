using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class SingleAddressCouch 
{
    public Guid? Id { get; set; }
    public string? NameAm { get; set; }
    public string? NameOr { get; set; }
    public int? AdminLevel {get;set;}
    public string? AdminTypeAm {get; set;}
    public string? AdminTypeOr {get; set;} 
    public Guid? ParentAddressId {get; set; }
    public bool? Status {get; set;}
}