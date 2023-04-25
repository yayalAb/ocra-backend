using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class ContactInfoDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? HouseNumber { get; set; }
        public string? Website { get; set; }
        public string? Linkdin { get; set; }
        // public Guid PersonalInfoId { get; set; }

    }
}