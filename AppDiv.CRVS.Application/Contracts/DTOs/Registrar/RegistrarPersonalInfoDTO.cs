using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs.Registrar
{
    public class RegistrarPersonalInfoDTO
    {
        public JObject FirstName { get; set; }
        public JObject MiddleName { get; set; }
        public JObject LastName { get; set; }
        public Guid AddressId { get; set; }
    }
}