using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDiv.CRVS.Application.Contracts.DTOs;

namespace AppDiv.CRVS.Application.Contracts.Request
{
    public class RegistrarForDeathRequest : AddRegistrarRequest
    {
        public virtual RegistrarPersonalInfoDTO RegistrarInfo { get; set; }
    }
}