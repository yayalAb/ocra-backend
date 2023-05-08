using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.HttpContext;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventRequest
    {
        public string EventType { get; set; }
        public Guid EventOwenerId { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventRegDate { get; set; }
        public Guid EventAddressId { get; set; }
        public AddRegistrarRequest RegistrarInfo { get; set; }
        public AddPaymentExamptionRequest PaymentExamption { get; set; }
        public Guid InformantTypeLookupId { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
        public bool IsCertified { get; set; } = false;
        public virtual AddPersonalInfoRequest EventOwener { get; set; }
        public ICollection<SupportingDocumentRequest> Attachments { get; set; }
    }
}