using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddDeathEventRequest
    {
        // public Guid Id { get; set; }
        public Guid FacilityTypeId { get; set; }
        public Guid FacilityId { get; set; }
        public string DuringDeath { get; set; }
        public string PlaceOfFuneral { get; set; }
        public AddDeathNotificationRequest DeathNotification { get; set; }
        public AddEventRequest Event { get; set; }

    }
}