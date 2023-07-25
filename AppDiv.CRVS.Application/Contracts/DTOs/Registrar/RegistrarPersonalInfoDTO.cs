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
        public Guid? SexLookupId { get; set; }
        public string? NationalId { get; set; }
        public Guid? ResidentAddressId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        
    }
}