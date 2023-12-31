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
    public class ReportStore : BaseAuditableEntity
    {
        public string? ReportName { get; set; }
        public string? ReportTitleStr { get; set; }
        public string? Description { get; set; }
        public string? DefualtColumns { get; set; }
        public string? Query { get; set; }
        public string? columnsLang { get; set; }
        public  string? UserGroupsStr { get; set; }
        public  bool? isAddressBased { get; set; }=false;
        public string? Other { get; set; }
        public Guid? ReportGroupId { get; set; }
        public virtual Lookup ReportGroup { get; set; }
        
        [NotMapped]
        public List<Guid>? UserGroups
        {
            get
            {
                return JsonConvert.DeserializeObject<List<Guid>>(string.IsNullOrEmpty(UserGroupsStr) ? "[]" : UserGroupsStr);
            }
            set
            {
                UserGroupsStr =(string.IsNullOrEmpty(value.ToString()) ||value.Count==0) ? "[]":JsonConvert.SerializeObject(value);
            }
        }
        [NotMapped]
        public JObject ReportTitle
        {
            get
            {
                return JsonConvert.DeserializeObject<JObject>(string.IsNullOrEmpty(ReportTitleStr) ? "{}" : ReportTitleStr);
            }
            set
            {
                ReportTitleStr = value.ToString();
            }
        }
        [NotMapped]
        public string? ReportTitleLang
        {
            get
            {
                return ReportTitle.Value<string>(lang);
            }
        }
    }
}