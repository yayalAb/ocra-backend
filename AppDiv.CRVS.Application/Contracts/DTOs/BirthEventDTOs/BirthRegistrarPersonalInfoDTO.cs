using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class BirthRegistrarPersonalInfoDTO : RegistrarPersonalInfoDTO
    {
        public Guid? SexLookupId { get; set; }
        // public DateTime BirthDate { get; set; }
        public string BirthDateEt { get; set; }
        public Guid? BirthAddressId { get; set; }
        public Guid? ReligionLookupId { get; set; }
        public Guid? NationalityLookupId { get; set; }
        public Guid? NationLookupId { get; set; }
       
    }
}