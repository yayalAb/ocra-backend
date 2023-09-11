using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddDeathEventRequest
    {
        public Guid? Id { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? DuringDeathId { get; set; }
        public Guid? DeathPlaceId { get; set; }
        public JObject? PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest? DeathNotification { get; set; } = null;
        public AddEventForDeathRequest Event { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }

    }
}