using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class GroomInfoDTO : PersonInfoDTO
    {
        public Guid BirthAddressId { get; set; }

    }
}