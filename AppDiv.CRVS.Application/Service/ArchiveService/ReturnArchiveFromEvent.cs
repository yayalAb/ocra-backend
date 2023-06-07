using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Service.ArchiveService
{
    public static class ReturnArchiveFromJObject
    {
        public static T GetArchive<T>(JObject claimedEvent)
        {
            T myEvent = claimedEvent.ToObject<T>();
            // GetBirthEvent(myEvent);

            return myEvent;
        }
        // public static Event GetBirthEvent(BirthEvent birth)
        // {

        // }
    }
}