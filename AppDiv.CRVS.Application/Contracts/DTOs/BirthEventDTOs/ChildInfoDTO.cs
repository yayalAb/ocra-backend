using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ChildInfoDTO
    {
        public Guid? Id { get; set; } = null;
        public LanguageModel FirstName { get; set; }
        public Guid? SexLookupId { get; set; }
        // public DateTime BirthDate { get; set; }
        // public string BirthDateEt { get; set; }
        public Guid? NationalityLookupId { get; set; }
        public Guid? BirthAddressId { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}