
using AppDiv.CRVS.Application.Contracts.Request;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class DeathEventCouch : BaseEventCouch
    {
        public Guid Id2 { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? DuringDeathId { get; set; }
        public Guid? DeathPlaceId { get; set; }
        public JObject? PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest? DeathNotification { get; set; } = null;
        public AddEventForDeathRequest Event { get; set; }
    }
}