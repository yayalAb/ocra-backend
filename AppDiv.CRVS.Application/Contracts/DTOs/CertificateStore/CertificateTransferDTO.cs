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
        public int From { get; set; }
        public int To { get; set; }
    }
}