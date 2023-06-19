using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class UserHistoryDTO
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? AssignedOffice { get; set; }
        public string? Role { get; set; }
        public List<UserHistoryListDTO>? Historys { get; set; }
    }
}