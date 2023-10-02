
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AuthenticationRequestListDTO
    {
        public Guid Id { get; set; }
        public string? RequestedBy { get; set; }
        public string? RequestType { get; set; }
        public string EventType { get; set; }
        public Guid? EventId { get; set; }
        public string? CertificateId { get; set; }
        public string? OwnerFullName { get; set; }
        public Guid? OfficerId { get; set; }
        public string? RequestDate { get; set; }
        public int? CurrentStep { get; set; }
        public int? NextStep { get; set; }
        public Guid? RequestId { get; set; }
        public string EventDate { get; set; } = string.Empty;
        public string EventRegDate { get; set; } = string.Empty;
        public string? ResponsbleGroup { get; set; }
        public Guid ResponsbleGroupId { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanApprove { get; set; }
        public bool? IsCertified { get; set; }
        public bool? HasPendingDocumentApproval { get; set; }
        public bool? IsReprint { get; set; }
        public string ? ActionBy { get; set; }
        public string ? UserGroups { get; set; }
        

    }
}