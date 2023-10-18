using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class SharedReportResponseDTO
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? UserRole { get; set; }
        public string? Email { get; set; }
        public Boolean Status { get; set; }
        public string? ReportTitle { get; set; }
        
    }
}