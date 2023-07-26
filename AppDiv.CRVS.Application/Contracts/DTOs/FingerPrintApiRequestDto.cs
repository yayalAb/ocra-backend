using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Models;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FingerPrintApiRequestDto
    {
        public string? clientKey { get; set; }
        public string? registrationID { get; set; }
        public BiometricImages? images { get; set; }


    }
}