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

        public ICollection<Step> Steps { get; set; }
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

    }
}