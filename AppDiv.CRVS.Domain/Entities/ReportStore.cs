using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Domain.Entities
{
    public class ReportStore : BaseAuditableEntity
    {
        public string? ReportName { get; set; }
        public string? ReportTitle { get; set; }
        public string? Description { get; set; }
        public string? DefualtColumns { get; set; }

    }
}