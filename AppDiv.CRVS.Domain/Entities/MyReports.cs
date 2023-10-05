using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class MyReports : BaseAuditableEntity
    {
        public Guid ReportOwnerId { get; set; }
        public string? ReportName { get; set; }
        public string? Description { get; set; }
        public string? ReportTitleStr { get; set; }
        public string? Agrgate { get; set; }
        public string? Filter { get; set; }
        public string? Colums { get; set; }
        public string? Other { get; set; }
        public bool? IsShared { get; set; }
        public Guid? SharedFrom { get; set; }
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