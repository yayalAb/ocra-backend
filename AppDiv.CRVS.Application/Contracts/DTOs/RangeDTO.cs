using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class RangeDTO
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
    }
}