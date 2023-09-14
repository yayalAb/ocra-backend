using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ReportStoreDTO
    {
        public Guid? Id { get; set; }
        public string? ReportName { get; set; }
        public string? ReportTitle { get; set; }
    }
}