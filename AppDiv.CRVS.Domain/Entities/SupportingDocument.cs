using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class SupportingDocument : BaseAuditableEntity
    {
        public Guid EventId { get; set; }
        public string Description { get; set; }
        public string DocumentUrl { get; set; }

        public virtual Event Event { get; set; }
    }
}