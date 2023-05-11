using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.BirthEventDTO
{
    public class BirthRegistrarPersonalInfoDTO
    {
        public Guid SexLookupId { get; set; }
        public DateTime BirthDate { get; set; }
    }
}