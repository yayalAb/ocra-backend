using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddBirthEventRequest
    {
        public Guid FatherId { get; set; }
        public Guid MotherId { get; set; }
        public Guid FacilityTypeLookupId { get; set; }
        public Guid FacilityLookupId { get; set; }
        public Guid BirthPlaceId { get; set; }
        public Guid TypeOfBirthLookupId { get; set; }
        public Guid EventId { get; set; }

        public virtual UpdatePersonalInfoRequest Father { get; set; }
        public virtual UpdatePersonalInfoRequest Mother { get; set; }
        public virtual AddEventRequest Event { get; set; }
        public virtual AddBirthNotificationRequest BirthNotification { get; set; }
    }
}