using AppDiv.CRVS.Application.Contracts.Request;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AdoptionDTO
    {

        public Guid BeforeAdoptionAddressId { get; set; }
        public JObject ApprovedName { get; set; }
        public JObject Reason { get; set; }
        public Guid AdoptiveMotherId { get; set; }
        public Guid AdoptiveFatherId { get; set; }
        public Guid CourtCaseId { get; set; }
        public Guid EventId { get; set; }
        public virtual UpdatePersonalInfoRequest AdoptiveMother { get; set; }
        public UpdatePersonalInfoRequest AdoptiveFather { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public virtual AddEventRequest Event { get; set; }
    }
}