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
        public Guid FacilityTypeId { get; set; }
        public Guid FacilityId { get; set; }
        public Guid BirthPlaceId { get; set; }
        public Guid TypeOfBirthId { get; set; }
        public Guid EventId { get; set; }
        public virtual UpdatePersonalInfoRequest Father { get; set; }
        public virtual UpdatePersonalInfoRequest Mother { get; set; }
        public virtual LookupDTO FacilityType { get; set; }
        public virtual LookupDTO Facility { get; set; }
        public virtual LookupDTO TypeOfBirth { get; set; }
        public virtual AddressDTO BirthPlace { get; set; }
        public virtual EventDTO Event { get; set; }
        public virtual BirthNotificationDTO BirthNotification { get; set; }
    }
}