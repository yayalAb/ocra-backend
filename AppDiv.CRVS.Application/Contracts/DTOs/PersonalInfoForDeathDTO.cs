using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AppDiv.CRVS.Application.Contracts.DTOs
{
    public class PersonalInfoForDeathDTO
    {
        public Guid? Id { get; set; }
        public JObject? FirstName { get; set; }
        public JObject? MiddleName { get; set; }
        public JObject? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Guid SexLookupId { get; set; }
        public Guid? PlaceOfBirthLookupId { get; set; }
        public Guid NationalityLookupId { get; set; }
    }
}