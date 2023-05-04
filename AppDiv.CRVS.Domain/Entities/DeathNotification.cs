using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class DeathNotification  : BaseAuditableEntity
    {
        public string CouseOfDeath { get; set; }
        public Guid CouseOfDeathInfoTypeId { get; set; }
        public string DeathNotificationSerialNo { get; set; }
        public Guid DeathEventId { get; set; }
        
        public virtual Lookup CouseOfDeathInfoType { get; set; }
        public virtual DeathEvent DeathEvent { get; set; }
    }
}