using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;
using AppDiv.CRVS.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class AddUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string UserImage { get; set; }

        public List<Guid> UserGroups { get; set; }
        // public string Password { get; set; }
        public AddPersonalInfoRequest PersonalInfo { get; set; }
    }
}