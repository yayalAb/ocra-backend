using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Base;

namespace AppDiv.CRVS.Domain.Entities
{
    public class CertificateHistory : BaseAuditableEntity
    {
        public string? Reason { get; set; }
        public string SrialNo { get; set; }
        public Guid CivilRegOfficerId { get; set; }
        public Guid CerteficateId { get; set; }
        public string? PrintType { get; set; }

    }
}