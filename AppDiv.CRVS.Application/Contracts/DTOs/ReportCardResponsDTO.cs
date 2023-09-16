using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ReportCardResponsDTO
    {

        public int Birth { get; set; }
        public int Death { get; set; }
        public int Marriage { get; set; }
        public int Divorce { get; set; }
        public int Adoption { get; set; }
        
    }
}