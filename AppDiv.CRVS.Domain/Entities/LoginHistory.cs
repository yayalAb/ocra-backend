using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class LoginHistory
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string EventType { get; set; }
        public string? IpAddress { get; set; }
        public string? Device { get; set; }
        public DateTime? EventDate { get; set; } = DateTime.Now;
        public virtual ApplicationUser User { get; set; }

    }
}