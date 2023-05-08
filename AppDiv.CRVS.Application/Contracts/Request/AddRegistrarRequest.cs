using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddRegistrarRequest
    {
        public Guid RelationshipId { get; set; }
        public Guid RegistrarInfoId { get; set; }
        // public Guid EventId { get; set; }
        // public virtual AddLookupRequest Relationship { get; set; }
        public virtual AddPersonalInfoRequest RegistrarInfo { get; set; }
    }
}