using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class PaymentExamptionRequest : BaseAuditableEntity
    {
        public string? ReasonStr { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullNAme { get; set; }
        public DateTime ExamptedDate { get; set; }
        public string ExamptedBy { get; set; }
        public string? NumberOfClient { get; set; }
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
        public virtual ICollection<PaymentExamption> ExamptionRequestNavigation { get; set; }


    }
}