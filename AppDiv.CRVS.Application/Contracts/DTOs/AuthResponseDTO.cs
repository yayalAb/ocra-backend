using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public record AuthResponseDTO
    {
        public string UserId { get; set; }
        public Guid? PersonalInfoId { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public List<Guid> GroupIds {get;set;}
        public List<RoleDto> Roles { get; set; }
    }
}
