using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FingerprintApiLoginResponseDto
    {
        public responseData responseData { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string responseCode { get; set; }
    }

    public class responseData
    {
        public string accessToken { get; set; }
        // public int expiresIn { get; set; }
        public string tokenType { get; set; }
    }
}