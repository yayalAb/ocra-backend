using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppDiv.CRVS.Domain.Entities;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public record UserResponseDTO
    {
        public string Id { get; set; }
        // public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public Guid? AddressId { get; set; }
        public bool Status { get; set; }
        public ICollection<GroupDTO> UserGroups { get; set; }
        // public Guid PersonalInfoId { get; set; }
        public PersonalInfoDTO PersonalInfo { get; set; }
        public string? AddressString { get; set; }
        public string CreatedDate { get; set; } = "";
        public string FullName { get; set; } = "";
        public string GroupName { get; set; } = "";
        public string AdminLevel { get; set; }
        public string AddressCode { get; set; }
        public bool? CanRegisterEvent { get; set; } 
        public DateTime? WorkStartedOn {get; set;}





    }
}
