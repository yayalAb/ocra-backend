using System.Runtime.Serialization;
using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace AppDiv.CRVS.Infrastructure.CouchModels;
public class LookupCouch : CouchDocument
{

    [DataMember]
    [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
    public Guid Id2 { get; set; }
    public string? Key { get; set; }
    public string? ValueAm { get; set; }
    public string? ValueOr { get; set; }
    public bool? status { get; set; }
}