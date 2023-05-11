using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs.DeathEventDTO
{
    public class DeadPersonalInfoDTO : PersonInfoDTO
    {
        public Guid AddressId { get; set; }
    }
}