using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public string? PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest? DeathNotification { get; set; } = null;
        public AddEventForDeathRequest Event { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public Guid? CreatedBy { get; set; }

    }
}