using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class RegistrarPersonalInfoDTO
    {
        public Guid? Id { get; set; } = null;
        public JObject FirstName { get; set; }
        public JObject MiddleName { get; set; }
        public JObject LastName { get; set; }
        public Guid SexLookupId { get; set; }
        public Guid ResidentAddressId { get; set; }
    }
}