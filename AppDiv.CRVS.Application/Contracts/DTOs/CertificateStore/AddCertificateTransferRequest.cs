using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class AddCertificateTransferRequest
    {
        public string? SenderId { get; set; }
        public string? ReceivedFrom { get; set; }
        public string RecieverId { get; set; }
        // public bool Status { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }

    public class AddDamagedCertificatesRequest
    {
        public string From { get; set; }
        public string To  { get; set; }
    }
}