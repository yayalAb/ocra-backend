using AppDiv.CRVS.Domain.Base;
using AppDiv.CRVS.Domain.Entities.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Domain.Entities
{
    public class UserGroup : BaseAuditableEntity
    {
        public string GroupName { get; set; }
        public string? DescriptionStr { get; set; }
        public string RolesStr { get; set; }
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
        public JArray Roles
        {
            get
            {
                return JsonConvert.DeserializeObject<JArray>(string.IsNullOrEmpty(RolesStr) ? "{}" : RolesStr);
            }
            set
            {
                RolesStr = value.ToString();
            }
        }
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
        public virtual ICollection<WorkerHistory> WorkerHistories { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
