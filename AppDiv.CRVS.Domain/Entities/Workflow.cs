using System.Collections.Generic;

using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Workflow : BaseAuditableEntity
    {
        public string workflowName { get; set; }
        public string DescriptionStr { get; set; }

        public virtual Request Request { get; set; }
        public ICollection<Step> Steps { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
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

        //  [NotMapped]
        // public string? ValueLang {
        //     get{
        //         return Value.Value<string>(lang) ;
        //     }
        // }

    }
}