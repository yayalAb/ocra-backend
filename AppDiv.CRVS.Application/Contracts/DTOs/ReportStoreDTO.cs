using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ReportStoreDTO
    {
        public Guid? Id { get; set; }
        public string? ReportName { get; set; }
        public string? ReportTitle { get; set; }
        public List<Guid>? Groups { get; set; }
        public string? ReportGroup { get; set; }

    }
}