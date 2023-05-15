using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class RegistrarPersonalInfoDTO
    {
        public Guid? Id { get; set; } = null;
        public LanguageModel FirstName { get; set; }
        public LanguageModel MiddleName { get; set; }
        public LanguageModel LastName { get; set; }
        public Guid SexLookupId { get; set; }
        public Guid ResidentAddressId { get; set; }
    }
}