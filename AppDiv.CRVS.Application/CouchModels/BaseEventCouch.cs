

using CouchDB.Driver.Types;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class BaseEventCouch : CouchDocument
    {
        public string EventType {get; set;}
        public bool Synced { get; set;}
        public bool? Updated {get; set;}
        public bool Certified {get; set;}
        public DateTime? CreatedAt {get; set;}
        
    }
}