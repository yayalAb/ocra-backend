
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Step : BaseAuditableEntity
    {
        public int step { get; set; }
        public bool Status { get; set; }
        public string DescriptionStr { get; set; }

        [NotMapped]
        public JObject Description
        {

            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(DescriptionStr) ? "{}" : DescriptionStr);
            }
            set
            {
                DescriptionStr = value.ToString();
            }
        }
        [NotMapped]
        public string DescriptionLang
        {
            get
            {
                return Description.Value<string>(lang);
            }
        }
        public Guid workflowId { get; set; }
        public Workflow workflow { get; set; }
        public virtual Guid? UserGroupId { get; set; }
        public virtual UserGroup UserGroup { get; set; }

    }
}