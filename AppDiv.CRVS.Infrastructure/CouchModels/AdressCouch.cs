using CouchDB.Driver.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class AddressCouch : CouchDocument
{
    public Guid? Id { get; set; }

    public string? NameAm
    {
        get
        {
            var NameObj = JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(NameStr) ? "{}" : NameStr);
            string value = NameObj["am"]?.Value<string>();
            return value;
        }
        set { }
    }
    public string? NameOr
    {
        get
        {
            var NameObj = JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(NameStr) ? "{}" : NameStr);
            string value = NameObj["or"]?.Value<string>();
            return value;
        }
        set { }
    }
    public string? NameStr { get; set; }
    public Guid? ParentAddressId { get; set; }
    public int? AdminLevel { get; set; }
    public string? AdminTypeAm { get; set; }
    public string? AdminTypeOr { get; set; }
    public bool? Status { get; set; }
    public bool DeletedStatus { get; set; }

    public List<SingleAddressCouch> addresses { get; set; }
}