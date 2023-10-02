using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AdoptionDTO
    {
        public Guid Id { get; set; }
        public Guid? BeforeAdoptionAddressId { get; set; }
        public string? BirthCertificateId { get; set; }
        public JObject ApprovedName { get; set; }
        public JObject? Reason { get; set; }
        public virtual AdoptionEventPersonalInfoDTO AdoptiveMother { get; set; }
        public AdoptionEventPersonalInfoDTO AdoptiveFather { get; set; }
        public virtual CourtCaseDTO? CourtCase { get; set; }
        public virtual AdoptionEventDTO Event { get; set; }
        public AddressResponseDTOE? BeforeAdoptionAddressResponseDTO { get; set; }
        public NotificationData? Comment { get; set; } = null;


    }
}