using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddBirthNotificationRequest
    {
        // public Guid BirthEventId { get; set; }
        public Guid DeliveryTypeId { get; set; }
        public float WeghtAtBirth { get; set; }
        public Guid SkilledProfId { get; set; }
        public string NotficationSerialNumber { get; set; }
    }
}