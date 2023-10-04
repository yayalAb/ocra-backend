using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class EventPlan : BaseAuditableEntity
    {
        public Guid PlanId { get; set; }
        public string EventType { get; set; }
        public uint ActiveTargetAmount { get; set; }
        public uint OtherTargetAmount { get; set; }
        public int ActualOccurance { get; set; }
        public string RemarkStr { get; set; } = string.Empty;

        public Plan Plan { get; set; }
        [NotMapped]
        public JObject Remark 
        { 
            get
            {
                return JObject.Parse(RemarkStr ?? "{}");
            } 
            set
            {
                RemarkStr = value.ToString();
            } 
        }
    }
}