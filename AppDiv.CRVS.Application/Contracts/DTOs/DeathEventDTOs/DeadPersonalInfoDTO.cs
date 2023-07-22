using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class DeadPersonalInfoDTO : PersonInfoDTO
    {
        public Guid? SexLookupId { get; set; }
        public string BirthDateEt { get; set; }
        public Guid? BirthAddressId { get; set; }
        public Guid? ResidentAddressId { get; set; }
        public Guid? TitleLookupId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}