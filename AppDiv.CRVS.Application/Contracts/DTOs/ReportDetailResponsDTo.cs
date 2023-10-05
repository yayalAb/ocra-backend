using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ReportDetailResponsDTo
    {
        public string? ReportName { get; set; }
        public JObject? ReportTitle { get; set; }
        public string? Description { get; set; }
        public string? DefualtColumns { get; set; }
        public string? Query { get; set; }
        public List<ReportColumsLngDto>? ColumnsLang { get; set; }
        public   List<Guid>? UserGroups { get; set; }
        public  bool? isAddressBased { get; set; }=false;
        public JObject? Other { get; set; }
        public string? ReportGroup { get; set; }
        
    }
}
