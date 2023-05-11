

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddDeathNotificationRequest
    {
        public string CauseOfDeath { get; set; }
        public Guid CauseOfDeathInfoTypeLookupId { get; set; }
        public string DeathNotificationSerialNumber { get; set; }
        // public Guid DeathEventId { get; set; }

        // public virtual Lookup CauseOfDeathInfoType { get; set; }
        // public virtual DeathEvent DeathEvent { get; set; }
    }
}