using CouchDB.Driver.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; 

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class SettingCouch : CouchDocument
{
    public Guid Id { get; set; }
    public string? Key { get; set; }
    public JObject? Value { get; set; }
    public bool? status { get; set; }
}