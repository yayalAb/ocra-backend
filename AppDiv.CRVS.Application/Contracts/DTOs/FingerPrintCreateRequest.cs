using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FingerPrintCreateRequest
    {
        public string clientKey { get; set; }
        public string registrationID
        {
            get; set;
        }
        public BiometricImages? images { get; set; }
    }
}