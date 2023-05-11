using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddAdoptionRequest
    {
        public Guid BeforeAdoptionAddressId { get; set; }
        public JObject ApprovedName { get; set; }
        public JObject Reason { get; set; }
        public virtual AdoptionEventPersonalInfoDTO AdoptiveMother { get; set; }
        public AdoptionEventPersonalInfoDTO AdoptiveFather { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public virtual AddAdoptionEventRequest Event { get; set; }
    }
}