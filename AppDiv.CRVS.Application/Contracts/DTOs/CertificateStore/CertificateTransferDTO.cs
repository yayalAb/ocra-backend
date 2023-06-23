using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class CertificateTransferDTO
    {
        public Guid Id { get; set; }
        public string? SenderName { get; set; }
        public string? RecieverName { get; set; }
        public bool Status { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}