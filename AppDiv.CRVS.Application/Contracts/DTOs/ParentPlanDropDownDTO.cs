using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ParentPlanDropdownDTO
    {
        public Guid Id { get; set; }
        public string? Plan { get; set; }
        public uint TargetAmount { get; set; }
    }
}