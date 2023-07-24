using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FingerPrintResponseDto
    {
        public string clientKey { get; set; }
        public int instanceID { get; set; }
        public string sequenceNo { get; set; }
        public string operationName { get; set; }
        public string operationStatus { get; set; }
        public string operationResult { get; set; }
        public string message { get; set; }

    }
}