using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace AppDiv.CRVS.Domain.Entities
{
    public class AdoptionEvent : BaseAuditableEntity
    {
        public Guid? BeforeAdoptionAddressId { get; set; }
        public string? BirthCertificateId { get; set; }
        [NotMapped]
        public AddressResponseDTOE? BeforeAdoptionAddressResponsDTO { get; set; }

        public Guid? AdoptiveMotherId { get; set; }
        public Guid? AdoptiveFatherId { get; set; }
        public Guid CourtCaseId { get; set; }
        public Guid EventId { get; set; }
        public string ApprovedNameStr { get; set; }
        public string? ReasonStr { get; set; }
        [NotMapped]
        public JObject? Reason
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ReasonStr) ? "{}" : ReasonStr);
            }
            set
            {
                ReasonStr = value?.ToString();
            }
        }
        [NotMapped]
        public JObject ApprovedName
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ApprovedNameStr) ? "{}" : ApprovedNameStr)!;
            }
            set
            {
                ApprovedNameStr = value.ToString();
            }
        }

        public virtual Address BeforeAdoptionAddress { get; set; }
        public virtual PersonalInfo AdoptiveMother { get; set; }
        public PersonalInfo AdoptiveFather { get; set; }
        public virtual CourtCase CourtCase { get; set; }
        public virtual Event Event { get; set; }


    }
}