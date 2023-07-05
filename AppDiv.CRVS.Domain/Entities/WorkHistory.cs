using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class WorkerHistory : BaseAuditableEntity
    {
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        
    }
}