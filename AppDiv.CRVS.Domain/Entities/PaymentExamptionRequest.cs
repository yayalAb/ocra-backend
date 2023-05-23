using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EthiopianCalendar;


namespace AppDiv.CRVS.Domain.Entities
{
    public class PaymentExamptionRequest : BaseAuditableEntity
    {
        public string? ReasonStr { get; set; }
        public string? ExamptedClientId { get; set; }
        public string? ExamptedClientFullName { get; set; }
        public DateTime? ExamptedDate { get; set; }
        public string? ExamptedDateEt { get; set; }
        public string ExamptedBy { get; set; }
        public int? NumberOfClient { get; set; }
        public Guid? AddressId { get; set; }
        public string? CertificateType { get; set; }
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
        public string? ReasonLang
        {
            get
            {
                return Reason.Value<string>(lang);
            }
        }
        public virtual ICollection<PaymentExamption> ExamptionRequestNavigation { get; set; }
        public virtual Address Address { get; set; }

        [NotMapped]
        public string? _ExamptedDateEt
        {
            get { return ExamptedDateEt; }
            set
            {
                ExamptedDateEt = value;
                ExamptedDate = new EthiopianDate(DateTime.Parse(ExamptedDateEt).Year, DateTime.Parse(ExamptedDateEt).Month, DateTime.Parse(ExamptedDateEt).Day).ToGregorianDate();
            }
        }


    }
}