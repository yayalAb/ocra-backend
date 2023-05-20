using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class BrideInfoDTO : UpdatePersonalInfoDTO
    {
        public Guid? BirthAddressId { get; set; }
        public DateTime BirthDate { get; set; }

    }
}