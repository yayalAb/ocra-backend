using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class UpdateDeathNotificationRequest
    {
        public Guid Id { get; set; }
        public string CauseOfDeath { get; set; }
        public Guid CauseOfDeathInfoTypeId { get; set; }
        public string DeathNotificationSerialNumber { get; set; }
    }
}