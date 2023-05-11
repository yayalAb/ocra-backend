using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class MotherInfoDTO : PersonInfoDTO
    {
        public DateTime? BirthDate { get; set; }
        public Guid? BirthAddressId { get; set; }
    }
}