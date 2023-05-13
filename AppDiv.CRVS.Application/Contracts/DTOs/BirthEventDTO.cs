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
        public Guid FatherId { get; set; }
        public Guid MotherId { get; set; }
        public Guid FacilityTypeLookupId { get; set; }
        public Guid FacilityLookupId { get; set; }
        public Guid BirthPlaceId { get; set; }
        public Guid TypeOfBirthLookupId { get; set; }
        public Guid EventId { get; set; }
        public virtual UpdatePersonalInfoRequest Father { get; set; }
        public virtual UpdatePersonalInfoRequest Mother { get; set; }
        public virtual UpdatePersonalInfoRequest Child { get; set; }
        public virtual LookupDTO FacilityTypeLookup { get; set; }
        public virtual LookupDTO FacilityLookup { get; set; }
        public virtual LookupDTO TypeOfBirthLookup { get; set; }
        public virtual AddressDTO BirthPlace { get; set; }
        public virtual EventDTO Event { get; set; }
        public virtual BirthNotificationDTO BirthNotification { get; set; }
    }
}