using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class UpdateDeathNotificationRequest
    {
        public Guid Id { get; set; }
        public JArray CauseOfDeathArray { get; set; }
        public Guid CauseOfDeathInfoTypeLookupId { get; set; }
        public string DeathNotificationSerialNumber { get; set; }
    }
}