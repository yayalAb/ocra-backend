using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PersonalInfoSearchDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }



    }
}