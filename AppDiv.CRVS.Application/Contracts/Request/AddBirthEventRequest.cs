using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddBirthEventRequest
    {

        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public Guid? BirthPlaceId { get; set; }
        public Guid? TypeOfBirthLookupId { get; set; }
        // public Guid EventId { get; set; }

        public virtual FatherInfoDTO Father { get; set; }
        public virtual MotherInfoDTO Mother { get; set; }
        public virtual AddEventForBirthRequest Event { get; set; }
        public virtual AddBirthNotificationRequest? BirthNotification { get; set; } = null;
    }
}