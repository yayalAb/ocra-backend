using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class DeathEventDTO
    {
        public Guid Id { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid FacilityTypeLookupId { get; set; }
        public Guid FacilityLookupId { get; set; }
        public Guid DeathPlaceId { get; set; }
        public Guid DuringDeathId { get; set; }
        public string PlaceOfFuneral { get; set; }
        public DeathNotificationDTO? DeathNotification { get; set; }
        public EventDTO Event { get; set; }
    }

    public class FetchDeathEventDTO
    {
        public Guid Id { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid FacilityTypeLookupId { get; set; }
        public Guid FacilityLookupId { get; set; }
        public Guid DuringDeathLookupId { get; set; }
        public string PlaceOfFuneral { get; set; }
        public DeathNotificationDTO? DeathNotification { get; set; }
        public EventDTO Event { get; set; }
    }
}