using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;
using static AppDiv.CRVS.Application.Contracts.Request.AdoptionPersonalINformationRequest;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddAdoptionRequest
    {
        public Guid? Id { get; set; }
        public Guid BeforeAdoptionAddressId { get; set; }
        public string? BirthCertificateId { get; set; }
        public LanguageModel ApprovedName { get; set; }
        public LanguageModel Reason { get; set; }
        public virtual AddAdoptionPersonalInfoRequest? AdoptiveMother { get; set; }
        public AddAdoptionPersonalInfoRequest? AdoptiveFather { get; set; }
        public virtual AddCourtCaseRequest CourtCase { get; set; }
        public virtual AddAdoptionEventRequest Event { get; set; }
    }
}