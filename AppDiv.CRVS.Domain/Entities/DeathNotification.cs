using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class DeathNotification  : BaseAuditableEntity
    {
        public string CauseOfDeath { get; set; }
        public Guid CauseOfDeathInfoTypeLookupId { get; set; }
        public string DeathNotificationSerialNumber { get; set; }
        public Guid DeathEventId { get; set; }
        
        public virtual Lookup CauseOfDeathInfoTypeLookup { get; set; }
        public virtual DeathEvent DeathEvent { get; set; }
    }
}