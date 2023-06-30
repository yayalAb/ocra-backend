using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public record AuthResponseDTO
    {
        public string? UserId { get; set; }
        public Guid? PersonalInfoId { get; set; }
        public string? Name { get; set; }
        public string? Token { get; set; }
        public List<Guid>? GroupIds { get; set; }
        public List<RoleDto>? Roles { get; set; }
        public bool isFirstTime { get; set; } = false;
        public bool isOtpExpired {get; set; } =false;
        public bool isOtpUnverified { get; set; } = false;
        public int? AdminLevel { get; set; }
        public Guid? AddressId { get; set; }
        public string? PreferedLanguage { get; set; }
    }
}
