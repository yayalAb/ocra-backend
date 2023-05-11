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
        public Guid DeliveryTypeLookupId { get; set; }
        public float WeightAtBirth { get; set; }
        public Guid SkilledProfLookupId { get; set; }
        public string NotficationSerialNumber { get; set; }
        public virtual LookupDTO DeliveryTypeLookup { get; set; }
        public virtual LookupDTO SkilledProfLookup { get; set; }
    }
}