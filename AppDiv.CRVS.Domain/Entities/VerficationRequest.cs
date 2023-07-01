using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class VerficationRequest : BaseAuditableEntity
    {
        public Guid EventId { get; set; }
        public Guid RequestId { get; set; }
        public virtual Event Event { get; set; }
        public virtual Request Request { get; set; }

    }
}