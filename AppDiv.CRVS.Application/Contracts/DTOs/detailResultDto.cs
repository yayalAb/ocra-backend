using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class detailResultDto
    {
        public int fingersScore { get; set; }
        public matchedFingersDto[] matchedFingers { get; set; }
        public int score { get; set; }
        public string id { get; set; }
    }
}