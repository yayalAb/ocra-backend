using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class AdoptionEvent : BaseAuditableEntity
    {
        public Guid BeforeAdoptionAddressId { get; set; }
        public Guid AdoptiveMotherId { get; set; }
        public Guid AdoptiveFatherId { get; set; }
        public Guid CourtCaseId { get; set; }
        public Guid EventId { get; set; }
        public string ApprovedNameStr { get; set; }


        public string ReasonStr { get; set; }
        [NotMapped]
        public JObject Reason
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ReasonStr) ? "{}" : ReasonStr);
            }
            set
            {
                ReasonStr = value.ToString();
            }
        }
        [NotMapped]
        public JObject ApprovedName
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ApprovedNameStr) ? "{}" : ApprovedNameStr);
            }
            set
            {
                ReasonStr = value.ToString();
            }
        }

        public Address BeforeAdoptionAddress { get; set; }
        public PersonalInfo AdoptiveMother { get; set; }
        public PersonalInfo AdoptiveFather { get; set; }
        public CourtCase CourtCase { get; set; }
        public Event Event { get; set; }


    }
}