using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class BirthNotificationDTO
    {
        public Guid Id { get; set; }
        public Guid BirthEventId { get; set; }
        public Guid DeliveryTypeId { get; set; }
        public float WeghtAtBirth { get; set; }
        public Guid SkilledProfId { get; set; }
        public string NotficationSerialNumber { get; set; }
        public virtual LookupDTO DeliveryType { get; set; }
        public virtual LookupDTO SkilledProf { get; set; }
    }
}