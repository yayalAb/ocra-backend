using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class DeathEventDTO
    {
        public Guid Id { get; set; }
        public LookupDTO FacilityType { get; set; }
        public LookupDTO Facility { get; set; }
        public string DuringDeath { get; set; }
        public string PlaceOfFuneral { get; set; }
        public DeathNotificationDTO DeathNotification { get; set; }
        public EventDTO Event { get; set; }
    }
}