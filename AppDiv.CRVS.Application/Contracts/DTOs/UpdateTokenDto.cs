using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class UpdateTokenDto
    {
        public bool number { get; set; }
        public bool otherCharacter { get; set; }
        public bool upperCase { get; set; }
        public bool lowerCase { get; set; }
        public string minLength { get; set; }
        public string maxLength { get; set; }
        public string clientAPIKey { get; set; }
        public string clientKey { get; set; }
        public string token { get; set; }
    }
}