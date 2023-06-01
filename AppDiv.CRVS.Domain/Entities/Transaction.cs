
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Transaction : BaseAuditableEntity
    {
       public string OldValueStr {get; set;}
       public string NewValueStr { get; set; }
       public Guid WorkflowId {get; set;}
       public Guid RequestId { get; set; }
       public Guid CivilRegOfficerId {get; set; }

       public virtual Workflow Workflow { get; set; }
       public virtual Request Request {get; set; }
       public virtual PersonalInfo CivilRegOfficer { get; set; }

       
       [NotMapped]
        public JObject OldValue
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(OldValueStr) ? "{}" : OldValueStr);
            }
            set
            {
                OldValueStr = value.ToString();
            }
        }
        [NotMapped]
        public JObject NewValue
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(NewValueStr) ? "{}" : NewValueStr);
            }
            set
            {
                NewValueStr = value.ToString();
            }
        }


    }
}