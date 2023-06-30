using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.Request;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public record FetchSingleUserResponseDTO
    {
        public string Id { get; set; }
        // public string FullName { get; set; }
        public Guid AddressId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public string PreferedLanguage {get; set; }

        public ICollection<Guid> UserGroups { get; set; }
        // public Guid PersonalInfoId { get; set; }
        public UpdatePersonalInfoRequest PersonalInfo { get; set; }
        // public ContactInfoDTO ContactInfo {get; set; }

    }
}
