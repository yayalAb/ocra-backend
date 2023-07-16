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
        public string? Agrgate { get; set; }
        public string? Filter { get; set; }
        public string? Colums { get; set; }
        public string? Other { get; set; }
        public bool? IsShared { get; set; }
        public Guid? SharedFrom { get; set; }
    }
}