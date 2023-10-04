using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddRequest
    {
        public Guid? Id { get; set; }
        public string RequestType { get; set; }
        public Guid? CivilRegOfficerId { get; set; }
        public int? currentStep { get; set; } = 0;
        public int? NextStep { get; set; } = 0;


    }
}