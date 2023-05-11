using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class DeathNotificationDTO
    {
        public string CauseOfDeath { get; set; }
        public Guid CauseOfDeathInfoTypeLookupId { get; set; }
        public string DeathNotificationSerialNumber { get; set; }
        // public Guid DeathEventId { get; set; }

        public virtual LookupDTO CauseOfDeathInfoType { get; set; }
        // public virtual DeathEventDTO DeathEvent { get; set; }
    }
}