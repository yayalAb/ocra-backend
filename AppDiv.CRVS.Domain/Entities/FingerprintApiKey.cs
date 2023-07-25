using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Domain.Entities
{
    public class FingerprintApiKey
    {
        public Guid Id { get; set; }
        public string clientAPIKey { get; set; }
        public string clientKey { get; set; }
        public string token { get; set; }
    }
}