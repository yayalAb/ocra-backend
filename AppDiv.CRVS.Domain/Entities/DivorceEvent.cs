using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Utility.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class DivorceEvent : BaseAuditableEntity
    {
        public Guid DivorcedWifeId { get; set; }
        public string? WifeBirthCertificateId { get; set; }
        public string? HusbandBirthCertificate { get; set; }
        public DateTime? DateOfMarriage { get; set; }
        public string? DateOfMarriageEt {get;set ;}
        public string DivorceReasonStr { get; set; }
        public Guid CourtCaseId { get; set; }
        public int NumberOfChildren { get; set; }
        public Guid EventId { get; set; }
        public virtual PersonalInfo DivorcedWife { get; set; }
        public virtual CourtCase CourtCase { get; set; }
        public virtual Event Event { get; set; }
        [NotMapped]
        public JObject DivorceReason
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(DivorceReasonStr) ? "{}" : DivorceReasonStr);
            }
            set
            {
                DivorceReasonStr = value.ToString();
            }
        }

        [NotMapped]
        public string? _DateOfMarriageEt
        {
            get { return DateOfMarriageEt; }
            set
            {
                DateOfMarriageEt = value;
                DateOfMarriage = DateOfMarriageEt ==null ?null: new CustomDateConverter(DateOfMarriageEt).gorgorianDate;

              
            }
        }

    }
}