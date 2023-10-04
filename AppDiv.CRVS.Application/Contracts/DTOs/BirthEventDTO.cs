using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class BirthEventDTO
    {
        public Guid Id { get; set; }
        public Guid? FacilityTypeLookupId { get; set; }
        public Guid? FacilityLookupId { get; set; }
        public string? Description { get; set; }
        public Guid? BirthPlaceId { get; set; }
        public Guid? TypeOfBirthLookupId { get; set; }
        public virtual UpdatePersonalInfoRequest Father { get; set; }
        public virtual UpdatePersonalInfoRequest Mother { get; set; }
        public virtual EventDTO Event { get; set; }
        public virtual BirthNotificationDTO? BirthNotification { get; set; }
        public NotificationData? Comment { get; set; } = null;
    }
}