using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class DeathEventDTO
    {
        public Guid Id { get; set; }
        public string? BirthCertificateId { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public string? Description { get; set; }
        public Guid? DeathPlaceId { get; set; }
        public Guid? DuringDeathId { get; set; }
        public string? PlaceOfFuneralStr { get; set; }
        public NotificationData? Comment { get; set; } = null;

        [NotMapped]
        public JObject? PlaceOfFuneral
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(PlaceOfFuneralStr) ? "{}" : PlaceOfFuneralStr);
            }
            set
            {
                PlaceOfFuneralStr = value.ToString();
            }
        }

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