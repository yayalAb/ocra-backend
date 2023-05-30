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
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? NationalId { get; set; }
        public string? NationalityLookup { get; set; }
        public string? TitleLookup { get; set; }
        public string? TypeOfWorkLookup { get; set; }
        public string? BirthDateEt { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }


    }
}