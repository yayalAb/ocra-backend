
using System.ComponentModel.DataAnnotations.Schema;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class Step : BaseAuditableEntity
    {
        public int step { get; set; }
        public string ResponsibleGroup { get; set; }
        public float Payment { get; set; }
        public float Status { get; set; }
        public string DescreptionStr { get; set; }

        [NotMapped]
        public JObject Descreption
        {

            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(DescreptionStr) ? "{}" : DescreptionStr);
            }
            set
            {
                DescreptionStr = value.ToString();
            }
        }
        public Guid workflowId { get; set; }
        public Workflow workflow { get; set; }

    }
}