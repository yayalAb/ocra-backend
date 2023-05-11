using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class FatherInfoDTO : PersonInfoDTO
    {
        public Guid BirthDate { get; set; }
        public Guid? BirthAddressId { get; set; }
    }
}