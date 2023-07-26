using System.Runtime.Serialization;
using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class CountryCouch : CouchDocument
{
    [DataMember]
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public Guid Id2 { get; set; }
    public string? NameAm { get; set; }
    public string? NameOr { get; set; }
    public bool? Status { get; set; }
    public bool DeletedStatus { get; set; }
}