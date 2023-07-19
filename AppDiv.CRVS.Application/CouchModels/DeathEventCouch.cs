
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.CouchModels
{
    public class DeathEventCouch : BaseEventCouch
    {
        public Guid Id { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? DuringDeathId { get; set; }
        public Guid? DeathPlaceId { get; set; }
        public string? PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest? DeathNotification { get; set; } = null;
        public AddEventForDeathRequest Event { get; set; }
    }
}