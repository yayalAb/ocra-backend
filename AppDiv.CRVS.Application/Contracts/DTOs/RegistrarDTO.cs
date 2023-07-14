using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class RegistrarDTO
    {
        public Guid Id { get; set; }
        public Guid? RelationshipLookupId { get; set; }
        public Guid RegistrarInfoId { get; set; }

        public virtual LookupDTO Relationship { get; set; }
        public virtual UpdatePersonalInfoRequest RegistrarInfo { get; set; }
    }
}