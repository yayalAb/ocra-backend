using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class UserHistoryListDTO
    {
        public string? EventType { get; set; }
        public string? Device { get; set; }
        public string? IpAddress { get; set; }

        public DateTime? Date { get; set; }

    }
}