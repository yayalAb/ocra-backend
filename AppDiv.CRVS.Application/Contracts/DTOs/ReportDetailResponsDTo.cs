using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ReportDetailResponsDTo
    {
        public string? ReportName { get; set; }
        public string? ReportTitle { get; set; }
        public string? Description { get; set; }
        public string? DefualtColumns { get; set; }
        public string? Query { get; set; }
         public List<ReportColumsLngDto>? ColumnsLang { get; set; }
        
    }
}
