using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.BirthEventDTO
{
    public class MotherInfoDTO : PersonInfoDTO
    {
        public DateTime? BirthDate { get; set; }
    }
}