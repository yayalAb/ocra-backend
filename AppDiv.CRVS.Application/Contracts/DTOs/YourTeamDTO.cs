using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class YourTeamDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? UserGroup { get; set; }
        public string? AddressName { get; set; }
        public Guid? ParentAddressId { get; set; }
        public bool Status { get; set; }
    }
}