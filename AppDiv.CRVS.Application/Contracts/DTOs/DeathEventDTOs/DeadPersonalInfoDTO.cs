using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class DeadPersonalInfoDTO : PersonInfoDTO
    {
        public string BirthDateEt { get; set; }
        public Guid ResidentAddressId { get; set; }
        public Guid TitleLookupId { get; set; }
    }
}