using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddEventRequest
    {
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EventRegDate { get; set; }
        public Guid EventAddressId { get; set; }

        public Guid InformantTypeLookupId { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public bool IsExampted { get; set; } = false;
         public  UpdatePersonalInfoRequest EventOwener { get; set; }
        public  AddRegistrarRequest EventRegistrar { get; set; }
        public  ICollection<AddSupportingDocumentRequest> EventSupportingDocuments { get; set; }
        public AddPaymentExamptionRequest? PaymentExamption { get; set; }
        




    }
}